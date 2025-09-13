
// Data seeder for initial setup

using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database;

public static class DbSeeder
{
    public static async Task SeedAsync(ContextDb context)
    {
        // Don't seed if data already exists
        if (await context.Barbers.AnyAsync())
            return;

        // Create sample barbers
        var barber1 = new Barber
        {
            Id = Guid.NewGuid(),
            Name = "Mike Johnson",
            Email = "mike@barbershop.com",
            Phone = "555-0101",
            Specialties = "Haircuts, Fades, Styling",
            IsActive = true
        };

        var barber2 = new Barber
        {
            Id = Guid.NewGuid(),
            Name = "Sarah Smith",
            Email = "sarah@barbershop.com",
            Phone = "555-0102",
            Specialties = "Haircuts, Beards, Color",
            IsActive = true
        };

        context.Barbers.AddRange(barber1, barber2);
        await context.SaveChangesAsync();

        // Create sample schedules
        var mike_schedule = new List<BarberSchedule>
        {
            new() { Id = Guid.NewGuid(), BarberId = barber1.Id, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(17, 0), IsWorking = true },
            new() { Id = Guid.NewGuid(), BarberId = barber1.Id, DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(17, 0), IsWorking = true },
            new() { Id = Guid.NewGuid(), BarberId = barber1.Id, DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(17, 0), IsWorking = true },
            new() { Id = Guid.NewGuid(), BarberId = barber1.Id, DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(17, 0), IsWorking = true },
            new() { Id = Guid.NewGuid(), BarberId = barber1.Id, DayOfWeek = DayOfWeek.Friday, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(17, 0), IsWorking = true },
            new() { Id = Guid.NewGuid(), BarberId = barber1.Id, DayOfWeek = DayOfWeek.Saturday, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(15, 0), IsWorking = true },
            new() { Id = Guid.NewGuid(), BarberId = barber1.Id, DayOfWeek = DayOfWeek.Sunday, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(17, 0), IsWorking = false }
        };

        var sarah_schedule = new List<BarberSchedule>
        {
            new() { Id = Guid.NewGuid(), BarberId = barber2.Id, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(18, 0), IsWorking = false },
            new() { Id = Guid.NewGuid(), BarberId = barber2.Id, DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(18, 0), IsWorking = true },
            new() { Id = Guid.NewGuid(), BarberId = barber2.Id, DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(18, 0), IsWorking = true },
            new() { Id = Guid.NewGuid(), BarberId = barber2.Id, DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(18, 0), IsWorking = true },
            new() { Id = Guid.NewGuid(), BarberId = barber2.Id, DayOfWeek = DayOfWeek.Friday, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(18, 0), IsWorking = true },
            new() { Id = Guid.NewGuid(), BarberId = barber2.Id, DayOfWeek = DayOfWeek.Saturday, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(16, 0), IsWorking = true },
            new() { Id = Guid.NewGuid(), BarberId = barber2.Id, DayOfWeek = DayOfWeek.Sunday, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(18, 0), IsWorking = false }
        };

        context.BarberSchedules.AddRange(mike_schedule);
        context.BarberSchedules.AddRange(sarah_schedule);
        await context.SaveChangesAsync();

        // Create sample customers
        var customers = new[]
        {
            new Customer { Id = Guid.NewGuid(), Name = "John Doe", Email = "john@example.com", Phone = "555-1001" },
            new Customer { Id = Guid.NewGuid(), Name = "Jane Smith", Email = "jane@example.com", Phone = "555-1002" },
            new Customer { Id = Guid.NewGuid(), Name = "Bob Wilson", Email = "bob@example.com", Phone = "555-1003" }
        };

        context.Customers.AddRange(customers);
        await context.SaveChangesAsync();
    }
}