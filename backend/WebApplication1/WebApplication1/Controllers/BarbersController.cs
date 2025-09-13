using AutoMapper;
using Database;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Controllers.DTOs;

namespace WebApplication1.Controllers;
[ApiController]
[Route("api/[controller]")]
public class BarbersController : ControllerBase
{
    private readonly ContextDb _context;
    private readonly IMapper _mapper;

    public BarbersController(ContextDb context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BarberDto>>> GetBarbers()
    {
        var barbers = await _context.Barbers
            .Include(b => b.WeeklySchedule)
            .Where(b => b.IsActive)
            .ToListAsync();

        return Ok(_mapper.Map<List<BarberDto>>(barbers));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BarberDto>> GetBarber(Guid id)
    {
        var barber = await _context.Barbers
            .Include(b => b.WeeklySchedule)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (barber == null) return NotFound();
        
        return Ok(_mapper.Map<BarberDto>(barber));
    }

    [HttpPost]
    public async Task<ActionResult<BarberDto>> CreateBarber(CreateBarberDto dto)
    {
        if (await _context.Barbers.AnyAsync(b => b.Email == dto.Email))
            return BadRequest("Email already exists");

        var barber = _mapper.Map<Barber>(dto);
        _context.Barbers.Add(barber);

        // Create default schedule
        var defaultSchedule = CreateDefaultSchedule(barber.Id);
        _context.BarberSchedules.AddRange(defaultSchedule);
        
        await _context.SaveChangesAsync();

        var createdBarber = await _context.Barbers
            .Include(b => b.WeeklySchedule)
            .FirstAsync(b => b.Id == barber.Id);

        return CreatedAtAction(nameof(GetBarber), new { id = barber.Id }, 
            _mapper.Map<BarberDto>(createdBarber));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBarber(Guid id, CreateBarberDto dto)
    {
        var barber = await _context.Barbers.FindAsync(id);
        if (barber == null) return NotFound();

        if (await _context.Barbers.AnyAsync(b => b.Email == dto.Email && b.Id != id))
            return BadRequest("Email already exists");

        _mapper.Map(dto, barber);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/schedule")]
    public async Task<IActionResult> UpdateBarberSchedule(Guid id, UpdateScheduleDto dto)
    {
        var barber = await _context.Barbers
            .Include(b => b.WeeklySchedule)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (barber == null) return NotFound();

        _context.BarberSchedules.RemoveRange(barber.WeeklySchedule);

        var newSchedules = _mapper.Map<List<BarberSchedule>>(dto.WeeklySchedule);
        foreach (var schedule in newSchedules)
            schedule.BarberId = id;

        _context.BarberSchedules.AddRange(newSchedules);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("available/{date}")]
    public async Task<ActionResult<IEnumerable<BarberDto>>> GetAvailableBarbers(DateTime date)
    {
        var dayOfWeek = date.DayOfWeek;
        var barbers = await _context.Barbers
            .Include(b => b.WeeklySchedule)
            .Where(b => b.IsActive && 
                       b.WeeklySchedule.Any(s => s.DayOfWeek == dayOfWeek && s.IsWorking))
            .ToListAsync();

        return Ok(_mapper.Map<List<BarberDto>>(barbers));
    }

    [HttpGet("{id}/slots/{date}")]
    public async Task<ActionResult<IEnumerable<DateTime>>> GetAvailableSlots(Guid id, DateTime date)
    {
        var barber = await _context.Barbers
            .Include(b => b.WeeklySchedule)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (barber == null) return NotFound();

        var daySchedule = barber.WeeklySchedule
            .FirstOrDefault(s => s.DayOfWeek == date.DayOfWeek && s.IsWorking);

        if (daySchedule == null) return Ok(new List<DateTime>());

        var start = date.Date.Add(daySchedule.StartTime.ToTimeSpan());
        var end = date.Date.Add(daySchedule.EndTime.ToTimeSpan());

        // Get existing appointments (avoiding computed properties)
        var existingAppointments = await _context.Appointments
            .Where(a => a.BarberId == id && 
                       a.StartTime.Date == date.Date && 
                       a.Status == AppointmentStatus.Booked)
            .Select(a => new { a.StartTime, a.DurationMinutes })
            .ToListAsync();

        var slots = new List<DateTime>();
        for (var time = start; time < end; time = time.AddMinutes(30))
        {
            var hasConflict = existingAppointments.Any(a => 
                time >= a.StartTime && time < a.StartTime.AddMinutes(a.DurationMinutes));

            if (!hasConflict) slots.Add(time);
        }

        return Ok(slots);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBarber(Guid id)
    {
        var barber = await _context.Barbers.FindAsync(id);
        if (barber == null) return NotFound();

        var hasFutureAppointments = await _context.Appointments
            .AnyAsync(a => a.BarberId == id && a.StartTime > DateTime.Now && a.Status == AppointmentStatus.Booked);

        if (hasFutureAppointments)
            return BadRequest("Cannot delete barber with future appointments");

        barber.IsActive = false;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private List<BarberSchedule> CreateDefaultSchedule(Guid barberId)
    {
        var schedule = new List<BarberSchedule>();
        for (int day = 0; day < 7; day++)
        {
            var dayOfWeek = (DayOfWeek)day;
            var isWeekend = dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;
            
            schedule.Add(new BarberSchedule
            {
                Id = Guid.NewGuid(),
                BarberId = barberId,
                DayOfWeek = dayOfWeek,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsWorking = !isWeekend
            });
        }
        return schedule;
    }
}