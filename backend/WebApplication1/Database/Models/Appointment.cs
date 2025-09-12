namespace Database.Models;

public class Appointment
{
    public Guid Id { get; set; }
    public DateTime DateAndTime { get; set; } 
    
    // Foreign key
    public Guid UserId { get; set; }
    
    // Navigation property
    public User User { get; set; }
}