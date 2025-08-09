namespace CRM_Backend.Models.DTOs.Appointment;

public class GetAllAppointmentsPaginationResponse
{
    public int Id { get; set; }
    public string PatientName { get; set; }
    public string Phone { get; set; }
    public string Service { get; set; }
    public string Date { get; set; }
    public string Time { get; set; }
    public string Hospital { get; set; }
    public string Status { get; set; }
}
