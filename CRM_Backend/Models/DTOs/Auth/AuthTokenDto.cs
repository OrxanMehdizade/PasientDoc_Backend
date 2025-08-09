namespace CRM_Backend.Models.DTOs.Auth;

public class AuthTokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ProfileImgUrl { get; set; } = string.Empty;
}
