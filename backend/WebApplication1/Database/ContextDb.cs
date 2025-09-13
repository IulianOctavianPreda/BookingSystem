using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Database;

public class ContextDb : DbContext
{
    private static string DbPath = System.IO.Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "source.db");
    
    public DbSet<Barber> Barbers { get; set; }
    public DbSet<BarberSchedule> BarberSchedules { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    
    public ContextDb(DbContextOptions<ContextDb> options) : base(options)
    {
      
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // Barber configuration
        modelBuilder.Entity<Barber>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
        });

        // BarberSchedule configuration
        modelBuilder.Entity<BarberSchedule>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Composite unique constraint: one schedule per barber per day
            entity.HasIndex(e => new { e.BarberId, e.DayOfWeek }).IsUnique();
            
            entity.HasOne(bs => bs.Barber)
                  .WithMany(b => b.WeeklySchedule)
                  .HasForeignKey(bs => bs.BarberId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            // Store TimeOnly as TEXT in SQLite
            entity.Property(e => e.StartTime)
                  .HasConversion(
                      v => v.ToString("HH:mm"),
                      v => TimeOnly.Parse(v));
                      
            entity.Property(e => e.EndTime)
                  .HasConversion(
                      v => v.ToString("HH:mm"),
                      v => TimeOnly.Parse(v));
        });

        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
        });

        // Appointment configuration
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Convert enum to string for SQLite
            entity.Property(e => e.Status)
                  .HasConversion<string>();
            
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
            
            // Customer relationship
            entity.HasOne(a => a.Customer)
                  .WithMany(c => c.Appointments)
                  .HasForeignKey(a => a.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            // Barber relationship
            entity.HasOne(a => a.Barber)
                  .WithMany(b => b.Appointments)
                  .HasForeignKey(a => a.BarberId)
                  .OnDelete(DeleteBehavior.Restrict); // Don't delete barber if they have appointments
            
            // Index for efficient queries
            entity.HasIndex(e => e.StartTime);
            entity.HasIndex(e => new { e.BarberId, e.StartTime });
            entity.HasIndex(e => new { e.CustomerId, e.StartTime });
        });

        base.OnModelCreating(modelBuilder);
    }

    public static void AddDbContext(IServiceCollection services, string? connectionString)
    {
        services.AddDbContext<ContextDb>(options =>
        {
            options.UseSqlite(connectionString ?? $"Data Source={DbPath}",  b => b.MigrationsAssembly("WebApplication1"));
        });
    }

    public static async Task Seed(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContextDb>();
        await context.Database.EnsureCreatedAsync();
        await DbSeeder.SeedAsync(context);
    }
}

