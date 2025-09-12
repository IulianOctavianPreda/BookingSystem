using Database;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController: ControllerBase
{
    private readonly Context _context ;

    public UsersController(Context context): base()
    {
        _context = context;
    }
    
    
    // Get: api/users
    [HttpGet]
    public async Task<List<User?>> Get()
    {
        return await _context.Users.Include(u => u.Appointments).ToListAsync();
    }

    // Get: api/users/:id
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<User>> GetUser(Guid id)
    {
        return await _context.Users.Include(u => u.Appointments).FirstOrDefaultAsync(x => x.Id == id);
    }
    
    //Post: api/users
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User u)
    {
        if (_context.Users.Any(user => user != null && user.Email == u.Email))
        {
            return BadRequest(new { message = "Email already exists", field = "email" });
        }

        var user = new User()
        {
            Id = Guid.NewGuid(),
            Name = u.Name,
            Email = u.Email
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
    
    
    // PUT: api/users/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, User userDto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        // Check if email already exists for another user
        if (await _context.Users.AnyAsync(u => u.Email == userDto.Email && u.Id != id))
        {
            return BadRequest("Email already exists");
        }

        user.Name = userDto.Name;
        user.Email = userDto.Email;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await UserExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/users/{id}/appointments
    [HttpGet("{id}/appointments")]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetUserAppointments(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        var appointments = await _context.Appointments
            .Where(a => a.UserId == id)
            .OrderBy(a => a.DateAndTime)
            .ToListAsync();

        return appointments;
    }

    private async Task<bool> UserExists(Guid id)
    {
        return await _context.Users.AnyAsync(e => e.Id == id);
    }
    
}