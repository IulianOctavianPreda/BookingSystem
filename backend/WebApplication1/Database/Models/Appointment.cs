using System.ComponentModel.DataAnnotations;

namespace Database.Models;


public class Appointment
{
    public Guid Id { get; set; }
    
    [Required]
    public DateTime StartTime { get; set; }
    
    [Required]
    [Range(15, 300)]
    public int DurationMinutes { get; set; } = 30;
    
    [Required]
    public Guid CustomerId { get; set; }
    
    [Required]
    public Guid BarberId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string ServiceType { get; set; } // "Haircut", "Beard", "Both"
    
    [Required]
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Booked;
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Customer Customer { get; set; }
    public Barber Barber { get; set; }
}

public enum AppointmentStatus
{
    Booked,
    Completed,
    Cancelled,
    NoShow
}