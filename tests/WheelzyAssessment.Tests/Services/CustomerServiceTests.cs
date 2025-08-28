using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WheelzyAssessment.Data;
using WheelzyAssessment.Models;
using WheelzyAssessment.Services;

namespace WheelzyAssessment.Tests.Services;

[TestClass]
public class CustomerServiceTests
{
    private WheelzyDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<WheelzyDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new WheelzyDbContext(options);
    }

    [TestMethod]
    public async Task UpdateCustomersBalanceByInvoicesAsync_ShouldUpdateBalances_WhenInvoicesProvided()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new CustomerService(context);

        var customer1 = new Customer { Id = 1, Name = "Customer 1", Balance = 1000 };
        var customer2 = new Customer { Id = 2, Name = "Customer 2", Balance = 500 };
        
        context.Customers.AddRange(customer1, customer2);
        await context.SaveChangesAsync();

        var invoices = new List<Invoice>
        {
            new() { CustomerId = 1, Total = 100 },
            new() { CustomerId = 1, Total = 50 },
            new() { CustomerId = 2, Total = 75 }
        };

        // Act
        await service.UpdateCustomersBalanceByInvoicesAsync(invoices);

        // Assert
        var updatedCustomer1 = await context.Customers.FindAsync(1);
        var updatedCustomer2 = await context.Customers.FindAsync(2);

        Assert.AreEqual(850, updatedCustomer1!.Balance); // 1000 - 150
        Assert.AreEqual(425, updatedCustomer2!.Balance); // 500 - 75
    }

    [TestMethod]
    public async Task UpdateCustomersBalanceByInvoicesAsync_ShouldHandleNullList_Gracefully()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new CustomerService(context);

        // Act & Assert - Should not throw
        await service.UpdateCustomersBalanceByInvoicesAsync(null!);
    }

    [TestMethod]
    public async Task UpdateCustomersBalanceByInvoicesAsync_ShouldSkipInvoicesWithNullCustomerId()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new CustomerService(context);

        var customer = new Customer { Id = 1, Name = "Customer 1", Balance = 1000 };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var invoices = new List<Invoice>
        {
            new() { CustomerId = 1, Total = 100 },
            new() { CustomerId = null, Total = 50 } // Should be skipped
        };

        // Act
        await service.UpdateCustomersBalanceByInvoicesAsync(invoices);

        // Assert
        var updatedCustomer = await context.Customers.FindAsync(1);
        Assert.AreEqual(900, updatedCustomer!.Balance); // Only first invoice applied
    }
}