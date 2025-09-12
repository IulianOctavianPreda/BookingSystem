using Database;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly Context _context;

    public AppointmentsController(Context context)
    {
        _context = context;
    }

    // GET: api/appointments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAppointments()
    {
        return await _context.Appointments
            .Include(a => a.User)
            .Select(a => new
            {
                a.Id,
                a.DateAndTime,
                a.UserId,
                User = new { a.User.Name, a.User.Email }
            })
            .OrderBy(a => a.DateAndTime)
            .ToListAsync();
    }

    // GET: api/appointments/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Appointment>> GetAppointment(Guid id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
            return NotFound();

        return appointment;
    }

    // POST: api/appointments
    [HttpPost]
    public async Task<ActionResult<Appointment>> CreateAppointment(CreateAppointmentDto appointmentDto)
    {
        // Verify user exists
        var user = await _context.Users.FindAsync(appointmentDto.UserId);
        if (user == null)
            return BadRequest("User not found");

        // Check if appointment time is in the future
        if (appointmentDto.DateAndTime <= DateTime.Now)
        {
            return BadRequest("Appointment must be scheduled for a future date and time");
        }

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            DateAndTime = appointmentDto.DateAndTime,
            UserId = appointmentDto.UserId
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        // Return appointment with user data
        var createdAppointment = await _context.Appointments
            .Include(a => a.User)
            .FirstAsync(a => a.Id == appointment.Id);

        createdAppointment.User = null;

        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, createdAppointment);
    }

    // PUT: api/appointments/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointment(Guid id, UpdateAppointmentDto appointmentDto)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound();

        // Check if new appointment time is in the future
        if (appointmentDto.DateAndTime <= DateTime.Now)
        {
            return BadRequest("Appointment must be scheduled for a future date and time");
        }

        appointment.DateAndTime = appointmentDto.DateAndTime;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await AppointmentExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/appointments/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound();

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/appointments/upcoming
    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<object>>> GetUpcomingAppointments()
    {
        var upcomingAppointments = await _context.Appointments
            .Include(a => a.User)
            .Where(a => a.DateAndTime > DateTime.Now)
            .Select(a => new
            {
                a.Id,
                a.DateAndTime,
                a.UserId,
                User = new { a.User.Name, a.User.Email }
            })
            .OrderBy(a => a.DateAndTime)
            .ToListAsync();

        return upcomingAppointments;
    }

    // GET: api/appointments/by-date/{date}
    [HttpGet("by-date/{date}")]
    public async Task<ActionResult<IEnumerable<object>>> GetAppointmentsByDate(DateTime date)
    {
        var appointments = await _context.Appointments
            .Include(a => a.User)
            .Where(a => a.DateAndTime.Date == date.Date)
            .Select(a => new
            {
                a.Id,
                a.DateAndTime,
                a.UserId,
                User = new { a.User.Name, a.User.Email }
            })
            .OrderBy(a => a.DateAndTime)
            .ToListAsync();

        return appointments;
    }

    private async Task<bool> AppointmentExists(Guid id)
    {
        return await _context.Appointments.AnyAsync(e => e.Id == id);
    }
}

// DTOs for Appointment operations
public class CreateAppointmentDto
{
    public DateTime DateAndTime { get; set; }
    public Guid UserId { get; set; }
}

public class UpdateAppointmentDto
{
    public DateTime DateAndTime { get; set; }
}