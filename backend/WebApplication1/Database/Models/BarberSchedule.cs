using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class BarberSchedule
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid BarberId { get; set; }
    
    [Required]
    public DayOfWeek DayOfWeek { get; set; }
    
    [Required]
    public TimeOnly StartTime { get; set; }
    
    [Required]
    public TimeOnly EndTime { get; set; }
    
    public bool IsWorking { get; set; } = true;
    
    // Navigation property
    public Barber Barber { get; set; }
}