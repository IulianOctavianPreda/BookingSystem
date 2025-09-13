using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Controllers.DTOs;

// Customer DTOs
public class CustomerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CustomerWithAppointmentsDto : CustomerDto
{
    public List<AppointmentSummaryDto> Appointments { get; set; } = new();
}

public class CreateCustomerDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }
}

// Barber DTOs
public class BarberDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public string? Specialties { get; set; }
    public bool IsActive { get; set; }
    public List<BarberScheduleDto> WeeklySchedule { get; set; } = new();
}

public class BarberScheduleDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public bool IsWorking { get; set; }
}

public class CreateBarberDto
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    public string? Phone { get; set; }
    public string? Specialties { get; set; }
}

public class UpdateScheduleDto
{
    public List<BarberScheduleDto> WeeklySchedule { get; set; } = new();
}

// Appointment DTOs
public class AppointmentDto
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public string ServiceType { get; set; }
    public string Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public CustomerDto Customer { get; set; }
    public BarberDto Barber { get; set; }
}

public class AppointmentSummaryDto
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string ServiceType { get; set; }
    public string Status { get; set; }
    public string BarberName { get; set; }
}

public class CreateAppointmentDto
{
    [Required]
    public DateTime StartTime { get; set; }
    
    [Required]
    public Guid CustomerId { get; set; }
    
    [Required]
    public Guid BarberId { get; set; }
    
    [Required]
    public string ServiceType { get; set; }
    
    public string? Notes { get; set; }
}