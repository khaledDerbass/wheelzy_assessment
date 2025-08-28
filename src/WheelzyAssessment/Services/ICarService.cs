using WheelzyAssessment.Models;

namespace WheelzyAssessment.Services;

public interface ICarService
{
    Task<List<dynamic>> GetCarWithCurrentQuoteAndStatusAsync();
}