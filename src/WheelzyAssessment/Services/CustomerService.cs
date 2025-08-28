using Microsoft.EntityFrameworkCore;
using WheelzyAssessment.Data;
using WheelzyAssessment.Models;

namespace WheelzyAssessment.Services;

public class CustomerService : ICustomerService
{
    private readonly WheelzyDbContext _context;

    public CustomerService(WheelzyDbContext context)
    {
        _context = context;
    }

    // Improved version from Question 3
    public async Task UpdateCustomersBalanceByInvoicesAsync(List<Invoice> invoices)
    {
        if (invoices == null || !invoices.Any())
            return;

        // Validate invoices have CustomerIds
        var validInvoices = invoices.Where(i => i.CustomerId.HasValue).ToList();
        if (!validInvoices.Any())
            return;

        // Get unique customer IDs to minimize database calls
        var customerIds = validInvoices.Select(i => i.CustomerId!.Value).Distinct().ToList();
        
        // Single query to get all relevant customers
        var customers = await _context.Customers
            .Where(c => customerIds.Contains(c.Id))
            .ToListAsync();
        
        // Group invoices by customer for bulk calculation
        var invoicesByCustomer = validInvoices
            .GroupBy(i => i.CustomerId!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Total));
        
        // Update balances in memory
        foreach (var customer in customers)
        {
            if (invoicesByCustomer.TryGetValue(customer.Id, out var totalInvoiceAmount))
            {
                customer.Balance -= totalInvoiceAmount;
            }
        }
        
        // Single SaveChanges call for all updates
        await _context.SaveChangesAsync();
    }
}
