using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using WheelzyAssessment.Data;
using WheelzyAssessment.Services;

namespace WheelzyAssessment;


class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        Console.WriteLine("Wheelzy Technical Assessment Solution");
        Console.WriteLine("====================================");

        // Demonstrate the solutions
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            // Test car service (Question 1)
            var carService = services.GetRequiredService<ICarService>();
            Console.WriteLine("1. Testing Car Service (SQL + EF Core)...");
            var carData = await carService.GetCarWithCurrentQuoteAndStatusAsync();
            Console.WriteLine($"   Retrieved {carData.Count} car records");

            // Test customer service (Question 3)
            var customerService = services.GetRequiredService<ICustomerService>();
            Console.WriteLine("2. Testing Customer Service (Optimized Invoice Processing)...");
            // This would test the improved method in a real scenario

            // Test order service (Question 4)
            var orderService = services.GetRequiredService<IOrderService>();
            Console.WriteLine("3. Testing Order Service (Dynamic Filtering)...");
            var orders = await orderService.GetOrdersAsync(DateTime.Now.AddDays(-30), DateTime.Now, null, null, true);
            Console.WriteLine($"   Retrieved {orders.Count} orders");

            // Test code processor (Question 5)
            var codeProcessor = services.GetRequiredService<CodeFileProcessor>();
            Console.WriteLine("4. Testing Code File Processor...");
            Console.WriteLine("   Code transformation methods ready");

            Console.WriteLine("\nAll services initialized successfully!");
            Console.WriteLine("Run the unit tests to see detailed functionality.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("Note: Database connection may not be configured. See appsettings.json");
        }
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<WheelzyDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DefaultConnection") ??
                        "Server=(localdb)\\mssqllocaldb;Database=WheelzyAssessment;Trusted_Connection=true;"));

                services.AddMemoryCache();
                services.AddScoped<ICarService, CarService>();
                services.AddScoped<ICustomerService, CustomerService>();
                services.AddScoped<IOrderService, OrderService>();
                services.AddSingleton<CodeFileProcessor>();
            });
}
