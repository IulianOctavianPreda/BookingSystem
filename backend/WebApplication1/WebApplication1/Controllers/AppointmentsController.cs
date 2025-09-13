using AutoMapper;
using Database;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Controllers.DTOs;

namespace WebApplication1.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly ContextDb _context;
    private readonly IMapper _mapper;

    public AppointmentsController(ContextDb context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments()
    {
        var appointments = await _context.Appointments
            .Include(a => a.Customer)
            .Include(a => a.Barber)
                .ThenInclude(b => b.WeeklySchedule)
            .OrderBy(a => a.StartTime)
            .ToListAsync();

        return Ok(_mapper.Map<List<AppointmentDto>>(appointments));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDto>> GetAppointment(Guid id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Customer)
            .Include(a => a.Barber)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null) return NotFound();
        
        return Ok(_mapper.Map<AppointmentDto>(appointment));
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> CreateAppointment(CreateAppointmentDto dto)
    {
        // Validation
        var customer = await _context.Customers.FindAsync(dto.CustomerId);
        if (customer == null) return BadRequest("Customer not found");

        var barber = await _context.Barbers
            .Include(b => b.WeeklySchedule)
            .FirstOrDefaultAsync(b => b.Id == dto.BarberId && b.IsActive);
        if (barber == null) return BadRequest("Barber not found or inactive");

        if (dto.StartTime <= DateTime.Now)
            return BadRequest("Appointment must be scheduled for a future date and time");

        // Business logic validation
        var dayOfWeek = dto.StartTime.DayOfWeek;
        var schedule = barber.WeeklySchedule.FirstOrDefault(s => s.DayOfWeek == dayOfWeek && s.IsWorking);
        if (schedule == null) return BadRequest("Barber is not working on this day");

        var appointmentTime = TimeOnly.FromDateTime(dto.StartTime);
        if (appointmentTime < schedule.StartTime || appointmentTime >= schedule.EndTime)
            return BadRequest("Appointment time is outside barber's working hours");

        // Check conflicts
        int duration = dto.ServiceType.ToLower() == "both" ? 60 : 30;
        var hasConflict = await _context.Appointments
            .AnyAsync(a => a.BarberId == dto.BarberId &&
                          a.Status == AppointmentStatus.Booked &&
                          ((dto.StartTime >= a.StartTime && dto.StartTime < a.StartTime.AddMinutes(a.DurationMinutes)) ||
                           (dto.StartTime.AddMinutes(duration) > a.StartTime && dto.StartTime.AddMinutes(duration) <= a.StartTime.AddMinutes(a.DurationMinutes))));

        if (hasConflict) return BadRequest("Time slot is not available");

        var appointment = _mapper.Map<Appointment>(dto);
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        var created = await _context.Appointments
            .Include(a => a.Customer)
            .Include(a => a.Barber)
            .FirstAsync(a => a.Id == appointment.Id);

        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, 
            _mapper.Map<AppointmentDto>(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointment(Guid id, CreateAppointmentDto dto)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound();

        if (appointment.StartTime <= DateTime.Now)
            return BadRequest("Cannot modify past or current appointments");

        _mapper.Map(dto, appointment);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateAppointmentStatus(Guid id, [FromBody] AppointmentStatus status)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound();

        appointment.Status = status;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound();

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("barber/{barberId}")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByBarber(Guid barberId)
    {
        var appointments = await _context.Appointments
            .Include(a => a.Customer)
            .Include(a => a.Barber)
            .Where(a => a.BarberId == barberId)
            .OrderBy(a => a.StartTime)
            .ToListAsync();

        return Ok(_mapper.Map<List<AppointmentDto>>(appointments));
    }

    [HttpGet("today")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetTodaysAppointments()
    {
        var today = DateTime.Now.Date;
        var appointments = await _context.Appointments
            .Include(a => a.Customer)
            .Include(a => a.Barber)
            .Where(a => a.StartTime.Date == today)
            .OrderBy(a => a.StartTime)
            .ToListAsync();

        return Ok(_mapper.Map<List<AppointmentDto>>(appointments));
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetUpcomingAppointments()
    {
        var appointments = await _context.Appointments
            .Include(a => a.Customer)
            .Include(a => a.Barber)
            .Where(a => a.StartTime > DateTime.Now && a.Status == AppointmentStatus.Booked)
            .OrderBy(a => a.StartTime)
            .Take(20)
            .ToListAsync();

        return Ok(_mapper.Map<List<AppointmentDto>>(appointments));
    }
}