using WheelzyAssessment.Models;

namespace WheelzyAssessment.Services;

public interface ICustomerService
{
    Task UpdateCustomersBalanceByInvoicesAsync(List<Invoice> invoices);
}