using Hangfire;
using Mcba.Services.Interfaces;
using Mcba.ViewModels.BillPay;
using McbaData;
using McbaData.Models;
using Microsoft.EntityFrameworkCore;
using static Mcba.Services.Interfaces.IBillPayService;

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
        localDT = new DateTime(localDT.Year, localDT.Month, localDT.Day, localDT.Hour, localDT.Minute, 0);
        var durationUntilNextPay = (newBillPayViewModel.ScheduleTimeLocal - localDT).Minutes;
        if (localDT.CompareTo(newBillPayViewModel.ScheduleTimeLocal) > 0)
        {
            durationUntilNextPay = 0;
        }

        var newBillPay = new BillPay()
        {
            AccountNumber = newBillPayViewModel.AccountNumber!.Value,
            PayeeID = newBillPayViewModel.PayeeID.GetValueOrDefault(),
            Amount = newBillPayViewModel.Amount.GetValueOrDefault(),
            ScheduleTimeUtc = newBillPayViewModel.ScheduleTimeLocal.ToUniversalTime(),
            Period = newBillPayViewModel.Period!.Value,
        };
        await _dbContext.BillPays.AddAsync(newBillPay);
        if (await _dbContext.SaveChangesAsync() > 0)
        {
            BackgroundJob.Schedule(
                () => PayBillPay(newBillPay.BillPayID, false),
                TimeSpan.FromMinutes(durationUntilNextPay));
        }
    }

    public async Task<BillPayError?> PayBillPay(int billPayID, bool isPayOverdue = false)
    {
        // Check if BillPay exists
        var billPay = await _dbContext.BillPays.FirstOrDefaultAsync(b => b.BillPayID == billPayID);
        if (billPay == null)
        {
            return BillPayError.NotExist;
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
            if (!isPayOverdue && billPay.Period == 'M')
            {
                var newBillPay = new BillPay()
                {
                    AccountNumber = billPay.AccountNumber,
                    PayeeID = billPay.PayeeID,
                    Amount = billPay.Amount,
                    ScheduleTimeUtc = billPay.ScheduleTimeUtc.AddMonths(1),
                    Period = billPay.Period,
                };
                if (await _dbContext.SaveChangesAsync() <= 0)
                {
                    return null;
                }

                var utcDT = DateTime.UtcNow;
                utcDT = new DateTime(utcDT.Year, utcDT.Month, utcDT.Day, utcDT.Hour, utcDT.Minute, 0);
                BackgroundJob.Schedule(
                    () => PayBillPay(newBillPay.BillPayID, false),
                    TimeSpan.FromMinutes((newBillPay.ScheduleTimeUtc - utcDT).TotalMinutes));
            }
            return BillPayError.InsuffientBalance;
        }
        else
        {
            _dbContext.BillPays.Remove(billPay);
            if (await _dbContext.SaveChangesAsync() <= 0)
            {
                return null;
            }

            await _dbContext.Transactions.AddAsync(new()
            {
                Amount = billPay.Amount,
                TransactionType = 'B',
                AccountNumber = billPay.AccountNumber,
                TransactionTimeUtc = billPay.ScheduleTimeUtc,
            });
            if (await _dbContext.SaveChangesAsync() <= 0)
            {
                return null;
            }

            if (!isPayOverdue && billPay.Period == 'M')
            {
                var newBillPay = new BillPay()
                {
                    AccountNumber = billPay.AccountNumber,
                    PayeeID = billPay.PayeeID,
                    Amount = billPay.Amount,
                    ScheduleTimeUtc = billPay.ScheduleTimeUtc.AddMonths(1),
                    Period = billPay.Period,
                };
                await _dbContext.AddAsync(newBillPay);
                if (await _dbContext.SaveChangesAsync() <= 0)
                {
                    return null;
                }

                var utcDT = DateTime.UtcNow;
                utcDT = new DateTime(utcDT.Year, utcDT.Month, utcDT.Day, utcDT.Hour, utcDT.Minute, 0);
                BackgroundJob.Schedule(
                    () => PayBillPay(newBillPay.BillPayID, false),
                    TimeSpan.FromMinutes((newBillPay.ScheduleTimeUtc - utcDT).TotalMinutes));
            }
        }
        return null;
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