using AutoMapper;
using Database;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Controllers.DTOs;

namespace WebApplication1.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ContextDb _context;
    private readonly IMapper _mapper;

    public CustomersController(ContextDb context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
    {
        var customers = await _context.Customers.OrderBy(c => c.Name).ToListAsync();
        return Ok(_mapper.Map<List<CustomerDto>>(customers));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerWithAppointmentsDto>> GetCustomer(Guid id)
    {
        var customer = await _context.Customers
            .Include(c => c.Appointments)
                .ThenInclude(a => a.Barber)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null) return NotFound();
        
        return Ok(_mapper.Map<CustomerWithAppointmentsDto>(customer));
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> CreateCustomer(CreateCustomerDto dto)
    {
        if (await _context.Customers.AnyAsync(c => c.Email == dto.Email))
            return BadRequest("Email already exists");

        var customer = _mapper.Map<Customer>(dto);
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, 
            _mapper.Map<CustomerDto>(customer));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(Guid id, CreateCustomerDto dto)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();

        if (await _context.Customers.AnyAsync(c => c.Email == dto.Email && c.Id != id))
            return BadRequest("Email already exists");

        _mapper.Map(dto, customer);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("search/{term}")]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> SearchCustomers(string term)
    {
        var customers = await _context.Customers
            .Where(c => c.Name.Contains(term) || c.Email.Contains(term) || c.Phone.Contains(term))
            .OrderBy(c => c.Name)
            .Take(10)
            .ToListAsync();

        return Ok(_mapper.Map<List<CustomerDto>>(customers));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}