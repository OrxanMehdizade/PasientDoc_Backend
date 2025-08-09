namespace CRM_Backend.Models.DTOs.Profile;

public class GetProfileDataResponse
{
    public string HospitalName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Specialization { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string WorkPhone { get; set; } = null!;
    public string ProfileImageUrl { get; set; } = null!;
    public TimeOnly? AppointmentDuration { get; set; }
}
