namespace CRM_Backend.Models.DTOs.Appointment;

public class AppointmentDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = null!;
}
