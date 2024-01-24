using Mcba.ViewModels.BillPay;

namespace Mcba.Services.Interfaces;

public interface IBillPayService
{
    public Task<List<BillPayViewModel>> GetBillPays();
    public Task AddBillPay(BillPayViewModel newBillPay);
    public Task PayBillPay(int billPayID);
    public Task DeleteBillPay(int billPayID);
}