using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class Barber
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; }
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    [MaxLength(500)]
    public string? Specialties { get; set; } // "Haircuts, Beards, Styling"
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public List<BarberSchedule> WeeklySchedule { get; set; } = new();
    public List<Appointment> Appointments { get; set; } = new();
}