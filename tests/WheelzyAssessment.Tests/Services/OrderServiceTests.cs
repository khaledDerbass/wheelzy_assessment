using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WheelzyAssessment.Data;
using WheelzyAssessment.Models;
using WheelzyAssessment.Services;

namespace WheelzyAssessment.Tests.Services;

[TestClass]
public class OrderServiceTests
{
    private WheelzyDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<WheelzyDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new WheelzyDbContext(options);
    }

    [TestMethod]
    public async Task GetOrdersAsync_ShouldReturnAllOrders_WhenNoFiltersApplied()
    {
        // Arrange
        using var context = GetInMemoryContext();
        await SeedTestDataAsync(context);
        var service = new OrderService(context);

        // Act
        var result = await service.GetOrdersAsync(null, null, null, null, null);

        // Assert
        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public async Task GetOrdersAsync_ShouldFilterByDateRange_WhenDatesProvided()
    {
        // Arrange
        using var context = GetInMemoryContext();
        await SeedTestDataAsync(context);
        var service = new OrderService(context);

        var dateFrom = DateTime.Today.AddDays(-5);
        var dateTo = DateTime.Today.AddDays(-1);

        // Act
        var result = await service.GetOrdersAsync(dateFrom, dateTo, null, null, null);

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result[0].OrderDate >= dateFrom && result[0].OrderDate <= dateTo);
    }

    [TestMethod]
    public async Task GetOrdersAsync_ShouldFilterByCustomerIds_WhenProvided()
    {
        // Arrange
        using var context = GetInMemoryContext();
        await SeedTestDataAsync(context);
        var service = new OrderService(context);

        var customerIds = new List<int> { 1 };

        // Act
        var result = await service.GetOrdersAsync(null, null, customerIds, null, null);

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(o => o.CustomerId == 1));
    }

    [TestMethod]
    public async Task GetOrdersAsync_ShouldFilterByActiveStatus_WhenProvided()
    {
        // Arrange
        using var context = GetInMemoryContext();
        await SeedTestDataAsync(context);
        var service = new OrderService(context);

        // Act
        var result = await service.GetOrdersAsync(null, null, null, null, true);

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(o => o.IsActive));
    }

    private async Task SeedTestDataAsync(WheelzyDbContext context)
    {
        var customers = new List<Customer>
        {
            new() { Id = 1, Name = "Customer 1" },
            new() { Id = 2, Name = "Customer 2" }
        };

        var statuses = new List<Status>
        {
            new() { Id = 1, Name = "Pending" },
            new() { Id = 2, Name = "Completed" }
        };

        var orders = new List<Order>
        {
            new() { Id = 1, CustomerId = 1, StatusId = 1, OrderDate = DateTime.Today.AddDays(-2), Total = 100, IsActive = true },
            new() { Id = 2, CustomerId = 1, StatusId = 2, OrderDate = DateTime.Today.AddDays(-10), Total = 200, IsActive = true },
            new() { Id = 3, CustomerId = 2, StatusId = 1, OrderDate = DateTime.Today.AddDays(-1), Total = 150, IsActive = false }
        };

        context.Customers.AddRange(customers);
        context.Statuses.AddRange(statuses);
        context.Orders.AddRange(orders);
        await context.SaveChangesAsync();
    }
}