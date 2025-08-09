using CRM_Backend.Models.Enums;

namespace CRM_Backend.Models.DTOs.Appointment;

public class UpdateAppointmentRequest
{
    public int PatientId { get; set; }
    public int ServiceId { get; set; }
    public string HospitalName { get; set; } = null!;
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public Appeal Appeal { get; set; }
    public string Status { get; set; } = null!;
}
