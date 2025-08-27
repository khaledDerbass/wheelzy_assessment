// EF LINQ example projection for Car summary (assumes DbContext with DbSet properties)
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WheelzyAssessment.Models;
public class CarSummaryDTO
{
    public int CarId { get; set; }
    public short Year { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public string Submodel { get; set; }
    public string Zip { get; set; }
    public string CurrentBuyer { get; set; }
    public decimal? CurrentQuote { get; set; }
    public string CurrentStatus { get; set; }
    public DateTime? CurrentStatusDate { get; set; }
}

public class ExampleQueries
{
    private readonly DbContext db;

    public ExampleQueries(DbContext context) => db = context;

    public async Task<CarSummaryDTO> GetCarSummary(int carId)
    {
        var q =
            from c in db.Set<Car>().AsNoTracking()
            join mk in db.Set<Make>()  on c.MakeId      equals mk.MakeId
            join mo in db.Set<Model>() on c.ModelId     equals mo.ModelId
            from sm in db.Set<SubModel>().Where(x => x.SubmodelId == c.SubmodelId).DefaultIfEmpty()
            from cq in db.Set<Quote>().Where(x => x.CarId == c.CarId && x.IsCurrent).DefaultIfEmpty()
            from cs in db.Set<Status>().Where(x => x.CarId == c.CarId && x.IsCurrent).DefaultIfEmpty()
            from b  in db.Set<Buyer>().Where(x => cq != null && x.BuyerId == cq.BuyerId).DefaultIfEmpty()
            from s  in db.Set<Status>().Where(x => cs != null && x.StatusId == cs.StatusId).DefaultIfEmpty()
            where c.CarId == carId
            select new CarSummaryDTO {
                CarId = c.CarId,
                Year = c.Year,
                Make = mk.Name,
                Model = mo.Name,
                Submodel = sm != null ? sm.Name : null,
                Zip = c.Zip,
                CurrentBuyer = b != null ? b.Name : null,
                CurrentQuote = cq != null ? cq.Amount : (decimal?)null,
                CurrentStatus = s != null ? s.Name : null,
                CurrentStatusDate = cs != null ? cs.StatusDate : (DateTime?)null
            };

        return await q.SingleOrDefaultAsync();
    }
}
