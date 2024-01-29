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
        List<BillPayViewModel> billPays = await (
            from bp in _dbContext.BillPays
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
            }
        ).ToListAsync();
        return billPays ?? [];
    }

    /// <summary>
    /// Add a new BillPay
    /// </summary>
    public async Task AddBillPay(BillPayViewModel newBillPayViewModel)
    {
        var localDT = DateTime.Now;
        localDT = new DateTime(
            localDT.Year,
            localDT.Month,
            localDT.Day,
            localDT.Hour,
            localDT.Minute,
            0
        );
        int durationUntilNextPay = (newBillPayViewModel.ScheduleTimeLocal - localDT).Minutes;
        if (localDT.CompareTo(newBillPayViewModel.ScheduleTimeLocal) > 0)
        {
            durationUntilNextPay = 0;
        }

        BillPay newBillPay = new BillPay()
        {
            AccountNumber = newBillPayViewModel.AccountNumber!.Value,
            PayeeID = newBillPayViewModel.PayeeID.GetValueOrDefault(),
            Amount = newBillPayViewModel.Amount.GetValueOrDefault(),
            ScheduleTimeUtc = newBillPayViewModel.ScheduleTimeLocal.ToUniversalTime(),
            Period = newBillPayViewModel.Period!.Value,
        };
        _ = await _dbContext.BillPays.AddAsync(newBillPay);
        if (await _dbContext.SaveChangesAsync() > 0)
        {
            _ = BackgroundJob.Schedule(
                () => PayBillPay(newBillPay.BillPayID),
                TimeSpan.FromMinutes(durationUntilNextPay)
            );
        }
    }

    /// <summary>
    /// Called when Add new BillPay 
    /// </summary>
    public async Task<BillPayError?> PayBillPay(int billPayID)
    {
        // Check if BillPay exists
        BillPay? billPay = await _dbContext.BillPays.FirstOrDefaultAsync(
            b => b.BillPayID == billPayID
        );
        if (billPay == null)
        {
            return BillPayError.NotExist;
        }

        // Verify the balance on account
        decimal totalBalance = await _balanceService.GetAccountBalance(billPay.AccountNumber);
        int minimumBalance =
            await (
                from a in _dbContext.Accounts
                where a.AccountNumber == billPay.AccountNumber
                select a.AccountType
            ).FirstOrDefaultAsync() == 'S'
                ? 0
                : 300;

        // Perform pay BillPay when amount is sufficient
        if ((totalBalance - billPay.Amount) >= minimumBalance)
        {
            // Remove BillPay
            _ = _dbContext.BillPays.Remove(billPay);
            if (await _dbContext.SaveChangesAsync() <= 0)
            {
                return null;
            }

            // Do BillPay transaction
            _ = await _dbContext.Transactions.AddAsync(
                new()
                {
                    Amount = billPay.Amount,
                    TransactionType = 'B',
                    AccountNumber = billPay.AccountNumber,
                    TransactionTimeUtc = billPay.ScheduleTimeUtc,
                }
            );
            if (await _dbContext.SaveChangesAsync() <= 0)
            {
                return null;
            }
        }

        // If monthly, create a bill due next month
        if (billPay.Period == 'M')
        {
            BillPay newBillPay =
                new()
                {
                    AccountNumber = billPay.AccountNumber,
                    PayeeID = billPay.PayeeID,
                    Amount = billPay.Amount,
                    ScheduleTimeUtc = billPay.ScheduleTimeUtc.AddMonths(1),
                    Period = billPay.Period,
                };
            _ = await _dbContext.AddAsync(newBillPay);
            if (await _dbContext.SaveChangesAsync() <= 0)
            {
                return null;
            }

            DateTime utcDT = DateTime.UtcNow;
            utcDT = new DateTime(
                utcDT.Year,
                utcDT.Month,
                utcDT.Day,
                utcDT.Hour,
                utcDT.Minute,
                0
            );
            _ = BackgroundJob.Schedule(
                () => PayBillPay(newBillPay.BillPayID),
                TimeSpan.FromMinutes((newBillPay.ScheduleTimeUtc - utcDT).TotalMinutes)
            );
        }
        return null;
    }

    /// <summary>
    /// Called to retry a failed payment.
    /// </summary>
    public async Task<BillPayError?> RetryBillPay(int billPayID)
    {
        // Check if BillPay exists
        BillPay? billPay = await _dbContext.BillPays.FirstOrDefaultAsync(
            b => b.BillPayID == billPayID
        );
        if (billPay == null)
        {
            return BillPayError.NotExist;
        }

        // Verify the amount of balance
        decimal totalBalance = await _balanceService.GetAccountBalance(billPay.AccountNumber);
        int minimumBalance =
            await (
                from a in _dbContext.Accounts
                where a.AccountNumber == billPay.AccountNumber
                select a.AccountType
            ).FirstOrDefaultAsync() == 'S'
                ? 0
                : 300;

        // Terminate if balance is insufficient
        if ((totalBalance - billPay.Amount) < minimumBalance)
        {
            return BillPayError.InsuffientBalance;
        }

        // Remove the BillPay
        _ = _dbContext.BillPays.Remove(billPay);
        if (await _dbContext.SaveChangesAsync() <= 0)
        {
            return null;
        }

        // Do BillPay transaction
        _ = await _dbContext.Transactions.AddAsync(
            new()
            {
                Amount = billPay.Amount,
                TransactionType = 'B',
                AccountNumber = billPay.AccountNumber,
                TransactionTimeUtc = DateTime.UtcNow,
            }
        );
        if (await _dbContext.SaveChangesAsync() <= 0)
        {
            return null;
        }

        return null;
    }

    public async Task DeleteBillPay(int billPayID)
    {
        BillPay? billPay = await _dbContext.BillPays.FirstOrDefaultAsync(
            b => b.BillPayID == billPayID
        );
        if (billPay != null)
        {
            _ = _dbContext.BillPays.Remove(billPay);
            _ = await _dbContext.SaveChangesAsync();
        }
    }
}

