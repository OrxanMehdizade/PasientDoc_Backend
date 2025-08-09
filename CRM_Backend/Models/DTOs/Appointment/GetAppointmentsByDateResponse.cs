namespace CRM_Backend.Models.DTOs.Appointment;

public class GetAppointmentsByDateResponse
{
    public int Id { get; set; }
    public string PatientFullName { get; set; }
    public string PhoneNumber { get; set; }
    public string ServiceName { get; set; }
    public string Time { get; set; }
    public string Status { get; set; }
}
