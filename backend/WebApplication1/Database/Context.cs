using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Database;

public class Context : DbContext
{
    private static string DbPath = System.IO.Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "source.db");
    
    public DbSet<User?> Users { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    
    public Context(DbContextOptions<Context> options) : base(options)
    {
      
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Appointment>(appointment =>
        {
            appointment.HasKey(e => e.Id);
            appointment.Property(e => e.DateAndTime).IsRequired();
            appointment.HasOne(e => e.User).WithMany(e => e.Appointments).HasForeignKey(e => e.UserId);
        });
        
        base.OnModelCreating(modelBuilder);
    }

    public static void AddDbContext(IServiceCollection services, string? connectionString)
    {
        services.AddDbContext<Context>(options =>
        {
            options.UseSqlite(connectionString ?? $"Data Source={DbPath}",  b => b.MigrationsAssembly("WebApplication1"));
        });
    }
}

