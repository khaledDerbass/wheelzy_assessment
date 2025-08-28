using Microsoft.EntityFrameworkCore;
using WheelzyAssessment.Models;

namespace WheelzyAssessment.Data;

public class WheelzyDbContext : DbContext
{
    public WheelzyDbContext(DbContextOptions<WheelzyDbContext> options) : base(options) { }

    public DbSet<Make> Makes { get; set; }
    public DbSet<Model> Models { get; set; }
    public DbSet<SubModel> SubModels { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Buyer> Buyers { get; set; }
    public DbSet<Quote> Quotes { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<CarStatusHistory> CarStatusHistory { get; set; }
    public DbSet<BuyerZipCode> BuyerZipCodes { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Make configuration
        modelBuilder.Entity<Make>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Model configuration
        modelBuilder.Entity<Model>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.Make).WithMany(e => e.Models).HasForeignKey(e => e.MakeId);
            entity.HasIndex(e => new { e.MakeId, e.Name }).IsUnique();
        });

        // SubModel configuration
        modelBuilder.Entity<SubModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.Model).WithMany(e => e.SubModels).HasForeignKey(e => e.ModelId);
            entity.HasIndex(e => new { e.ModelId, e.Name }).IsUnique();
        });

        // Car configuration
        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ZipCode).IsRequired().HasMaxLength(10);
            entity.HasOne(e => e.Make).WithMany(e => e.Cars).HasForeignKey(e => e.MakeId);
            entity.HasOne(e => e.Model).WithMany(e => e.Cars).HasForeignKey(e => e.ModelId);
            entity.HasOne(e => e.SubModel).WithMany(e => e.Cars).HasForeignKey(e => e.SubModelId);
        });

        // Quote configuration
        modelBuilder.Entity<Quote>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(10,2)");
            entity.HasOne(e => e.Car).WithMany(e => e.Quotes).HasForeignKey(e => e.CarId);
            entity.HasOne(e => e.Buyer).WithMany(e => e.Quotes).HasForeignKey(e => e.BuyerId);
        });

        // Status configuration
        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // CarStatusHistory configuration
        modelBuilder.Entity<CarStatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ChangedBy).HasMaxLength(100);
            entity.HasOne(e => e.Car).WithMany(e => e.StatusHistory).HasForeignKey(e => e.CarId);
            entity.HasOne(e => e.Status).WithMany(e => e.CarStatusHistories).HasForeignKey(e => e.StatusId);
        });

        // BuyerZipCode configuration
        modelBuilder.Entity<BuyerZipCode>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ZipCode).IsRequired().HasMaxLength(10);
            entity.HasOne(e => e.Buyer).WithMany(e => e.CoverageAreas).HasForeignKey(e => e.BuyerId);
            entity.HasIndex(e => new { e.BuyerId, e.ZipCode }).IsUnique();
        });

        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
        });

        // Invoice configuration
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Total).HasColumnType("decimal(10,2)");
            entity.HasOne(e => e.Customer).WithMany(e => e.Invoices).HasForeignKey(e => e.CustomerId);
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Total).HasColumnType("decimal(10,2)");
            entity.HasOne(e => e.Customer).WithMany(e => e.Orders).HasForeignKey(e => e.CustomerId);
            entity.HasOne(e => e.Status).WithMany().HasForeignKey(e => e.StatusId);
        });
    }
}
