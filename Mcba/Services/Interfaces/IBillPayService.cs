using Mcba.ViewModels.BillPay;

namespace Mcba.Services.Interfaces;

public interface IBillPayService
{
    public enum BillPayError {
        NotExist,
        InsuffientBalance,
    }

    public Task<List<BillPayViewModel>> GetBillPays();
    public Task AddBillPay(BillPayViewModel newBillPay);
    public Task<BillPayError?> PayBillPay(int billPayID);
    public Task<BillPayError?> RetryBillPay(int billPayID);
    public Task DeleteBillPay(int billPayID);
}