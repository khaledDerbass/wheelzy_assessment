using WheelzyAssessment.Models;

namespace WheelzyAssessment.Services;

public interface IOrderService
{
    Task<List<OrderDTO>> GetOrdersAsync(DateTime? dateFrom, DateTime? dateTo, List<int>? customerIds, List<int>? statusIds, bool? isActive);
}
