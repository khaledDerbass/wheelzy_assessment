// Refactored methods: UpdateCustomersBalanceByInvoicesAsync and GetOrders
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class Invoice { public int? CustomerId { get; set; } public decimal Total { get; set; } }
public class Customer { public int CustomerId { get; set; } public decimal Balance { get; set; } }
public class Order { public int OrderId { get; set; } public DateTime CreatedUtc { get; set; } public int CustomerId { get; set; }
    public Customer Customer { get; set; } public int StatusId { get; set; } public Status Status { get; set; } public bool IsActive { get; set; } public decimal Total { get; set; } }
public class Status { public int StatusId { get; set; } public string Name { get; set; } }
public record OrderDTO(int OrderId, DateTime CreatedUtc, int CustomerId, string CustomerName, int StatusId, string StatusName, bool IsActive, decimal Total);

public class RefactorExamples
{
    private readonly DbContext dbContext;
    public RefactorExamples(DbContext ctx) => dbContext = ctx;

    public async Task UpdateCustomersBalanceByInvoicesAsync(IEnumerable<Invoice> invoices, CancellationToken ct = default)
    {
        if (invoices is null) throw new ArgumentNullException(nameof(invoices));

        var deltas = invoices
            .Where(i => i?.CustomerId != null)
            .GroupBy(i => i.CustomerId!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Total));

        if (deltas.Count == 0) return;

        using var tx = await dbContext.Database.BeginTransactionAsync(ct);

        var ids = deltas.Keys.ToList();
        var customers = await dbContext.Set<Customer>()
            .Where(c => ids.Contains(c.CustomerId))
            .ToListAsync(ct);

        foreach (var c in customers)
        {
            var delta = deltas[c.CustomerId];
            c.Balance -= delta;
        }

        await dbContext.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    public async Task<List<OrderDTO>> GetOrders(DateTime? dateFrom, DateTime? dateTo, List<int> customerIds, List<int> statusIds, bool? isActive)
    {
        DateTime? toInclusive = dateTo.HasValue ? dateTo.Value : (DateTime?)null;

        IQueryable<Order> q = dbContext.Set<Order>().AsNoTracking();

        if (dateFrom.HasValue) q = q.Where(o => o.CreatedUtc >= dateFrom.Value);
        if (toInclusive.HasValue) q = q.Where(o => o.CreatedUtc <= toInclusive.Value);
        if (customerIds != null && customerIds.Count > 0) q = q.Where(o => customerIds.Contains(o.CustomerId));
        if (statusIds != null && statusIds.Count > 0) q = q.Where(o => statusIds.Contains(o.StatusId));
        if (isActive.HasValue) q = q.Where(o => o.IsActive == isActive.Value);

        var result = await q
            .OrderByDescending(o => o.CreatedUtc)
            .Select(o => new OrderDTO(o.OrderId, o.CreatedUtc, o.CustomerId, o.Customer.Name, o.StatusId, o.Status.Name, o.IsActive, o.Total))
            .ToListAsync();

        return result;
    }
}
