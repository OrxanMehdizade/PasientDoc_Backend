namespace CRM_Backend.Models.DTOs.Profile;

public class ChangePasswordRequest
{
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
}
