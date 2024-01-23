using Hangfire;
using Mcba.Data;
using Mcba.Models;
using Mcba.Services.Interfaces;
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
                                  ScheduleTimeUtc = bp.ScheduleTimeUtc
                              }).ToListAsync();
        return billPays ?? [];
    }

    public async Task AddBillPay(BillPayViewModel newBillPayViewModel)
    {
        var durationUntilNextPay = (newBillPayViewModel.ScheduleTimeUtc - DateTime.UtcNow).TotalMinutes;
        if (durationUntilNextPay < 1)
        {
            return;
        }

        var newBillPay = new BillPay()
        {
            AccountNumber = newBillPayViewModel.AccountNumber,
            PayeeID = newBillPayViewModel.PayeeID,
            Amount = newBillPayViewModel.Amount,
            ScheduleTimeUtc = newBillPayViewModel.ScheduleTimeUtc,
            Period = newBillPayViewModel.Period,
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