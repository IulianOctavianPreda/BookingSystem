namespace Database.Models;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public List<Appointment> Appointments { get; set; } = new();
}