using Hangfire;
using Mcba.Data;
using Mcba.Models;
using Mcba.Services.Interfaces;
using Mcba.ViewModels.BillPay;
using Microsoft.EntityFrameworkCore;

namespace Mcba.Services;

public class BillPayService(McbaContext dbContext, IBalanceService balanceService) : IBillPayService
{
    private readonly McbaContext _dbContext = dbContext;
    private readonly IBalanceService _balanceService = balanceService;

    public async Task<List<BillPayViewModel>> GetBillPays()
    {
        var billPays = await (from bp in _dbContext.BillPays
                              join p in _dbContext.Payees on bp.PayeeID equals p.PayeeID
                              select new BillPayViewModel
                              {
                                  BillPayID = bp.BillPayID,
                                  AccountNumber = bp.AccountNumber,
                                  Amount = bp.Amount,
                                  PayeeID = bp.PayeeID,
                                  PayeeName = p.Name,
                                  Period = bp.Period,
                                  ScheduleTimeLocal = bp.ScheduleTimeUtc.ToLocalTime()
                              }).ToListAsync();
        return billPays ?? [];
    }

    public async Task AddBillPay(BillPayViewModel newBillPayViewModel)
    {
        var localDT = DateTime.Now;
        var durationUntilNextPay = (newBillPayViewModel.ScheduleTimeLocal - localDT).Minutes;
        if (localDT.CompareTo(newBillPayViewModel.ScheduleTimeLocal) > 0)
        {
            durationUntilNextPay = 0;
        }

        var newBillPay = new BillPay()
        {
            AccountNumber = newBillPayViewModel.AccountNumber!.Value,
            PayeeID = newBillPayViewModel.PayeeID,
            Amount = newBillPayViewModel.Amount,
            ScheduleTimeUtc = newBillPayViewModel.ScheduleTimeLocal.ToUniversalTime(),
            Period = newBillPayViewModel.Period!.Value,
        };
        await _dbContext.BillPays.AddAsync(newBillPay);
        if (await _dbContext.SaveChangesAsync() > 0)
        {
            BackgroundJob.Schedule(
                () => PayBillPay(newBillPay.BillPayID),
                TimeSpan.FromSeconds(durationUntilNextPay));
        }
    }

    public async Task PayBillPay(int billPayID)
    {
        // Check if BillPay exists
        var billPay = await _dbContext.BillPays.FirstOrDefaultAsync(b => b.BillPayID == billPayID);
        if (billPay == null)
        {
            return;
        }

        var totalBalance = await _balanceService.GetAccountBalance(billPay.AccountNumber);
        int minimumBalance = await (
                from a in _dbContext.Accounts
                where a.AccountNumber == billPay.AccountNumber
                select a.AccountType
            ).FirstOrDefaultAsync() == 'S'
                ? 0
                : 300;
        if ((totalBalance - billPay.Amount) < minimumBalance)
        {
            if (billPay.Period == 'M')
            {
                billPay.ScheduleTimeUtc = billPay.ScheduleTimeUtc.AddMonths(1);
            }
            return;
        }
        else
        {
            await _dbContext.Transactions.AddAsync(new()
            {
                Amount = billPay.Amount,
                TransactionType = 'B',
                AccountNumber = billPay.AccountNumber,
                TransactionTimeUtc = billPay.ScheduleTimeUtc,
            });
            if (await _dbContext.SaveChangesAsync() <= 0)
            {
                return;
            }


            if (billPay.Period == 'M')
            {
                billPay.ScheduleTimeUtc = billPay.ScheduleTimeUtc.AddMonths(1);
            }
            else
            {
                _dbContext.BillPays.Remove(billPay);
            }
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteBillPay(int billPayID)
    {
        var billPay = await _dbContext.BillPays.FirstOrDefaultAsync(b => b.BillPayID == billPayID);
        if (billPay != null)
        {
            _dbContext.BillPays.Remove(billPay);
            await _dbContext.SaveChangesAsync();
        }
    }
}