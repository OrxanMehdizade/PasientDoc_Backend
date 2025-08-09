namespace CRM_Backend.Models.DTOs.Profile;

public class UpdateProfileRequest
{
    public string? Email { get; set; }
    public string? WorkPhone { get; set; }
    public string? HospitalName { get; set; }
    public string? Specialization { get; set; }
    public string? ProfilePictureImageUrl { get; set; }
}
