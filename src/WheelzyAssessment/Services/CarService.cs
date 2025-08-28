using Microsoft.EntityFrameworkCore;
using WheelzyAssessment.Data;

namespace WheelzyAssessment.Services;

public class CarService : ICarService
{
    private readonly WheelzyDbContext _context;

    public CarService(WheelzyDbContext context)
    {
        _context = context;
    }

    public async Task<List<dynamic>> GetCarWithCurrentQuoteAndStatusAsync()
    {
        // Entity Framework implementation from the assessment
        var result = await _context.Cars
            .Include(c => c.Make)
            .Include(c => c.Model)
            .Include(c => c.SubModel)
            .Include(c => c.Quotes.Where(q => q.IsCurrent))
                .ThenInclude(q => q.Buyer)
            .Include(c => c.StatusHistory.Where(sh => sh.IsCurrent))
                .ThenInclude(sh => sh.Status)
            .Select(c => new
            {
                c.Year,
                Make = c.Make.Name,
                Model = c.Model.Name,
                SubModel = c.SubModel != null ? c.SubModel.Name : null,
                c.ZipCode,
                CurrentBuyerName = c.Quotes.FirstOrDefault(q => q.IsCurrent)!.Buyer.Name,
                CurrentQuote = c.Quotes.FirstOrDefault(q => q.IsCurrent)!.Amount,
                CurrentStatusName = c.StatusHistory.FirstOrDefault(sh => sh.IsCurrent)!.Status.Name,
                StatusDate = c.StatusHistory.FirstOrDefault(sh => sh.IsCurrent)!.StatusDate
            })
            .ToListAsync();

        return result.Cast<dynamic>().ToList();
    }
}
