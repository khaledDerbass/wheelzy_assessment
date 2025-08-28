using Microsoft.EntityFrameworkCore;
using WheelzyAssessment.Data;
using WheelzyAssessment.Models;

namespace WheelzyAssessment.Services;

public class OrderService : IOrderService
{
    private readonly WheelzyDbContext _context;

    public OrderService(WheelzyDbContext context)
    {
        _context = context;
    }

    // Implementation from Question 4
    public async Task<List<OrderDTO>> GetOrdersAsync(
        DateTime? dateFrom, 
        DateTime? dateTo, 
        List<int>? customerIds, 
        List<int>? statusIds, 
        bool? isActive)
    {
        var query = _context.Orders.AsQueryable();
        
        // Apply filters conditionally - EF Core will optimize the query
        if (dateFrom.HasValue)
            query = query.Where(o => o.OrderDate >= dateFrom.Value);
        
        if (dateTo.HasValue)
            query = query.Where(o => o.OrderDate <= dateTo.Value);
        
        if (customerIds != null && customerIds.Any())
            query = query.Where(o => customerIds.Contains(o.CustomerId));
        
        if (statusIds != null && statusIds.Any())
            query = query.Where(o => statusIds.Contains(o.StatusId));
        
        if (isActive.HasValue)
            query = query.Where(o => o.IsActive == isActive.Value);
        
        // Project to DTO to select only needed columns
        var result = await query
            .Select(o => new OrderDTO
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer.Name,
                StatusId = o.StatusId,
                StatusName = o.Status.Name,
                Total = o.Total,
                IsActive = o.IsActive
            })
            .ToListAsync();
        
        return result;
    }
}