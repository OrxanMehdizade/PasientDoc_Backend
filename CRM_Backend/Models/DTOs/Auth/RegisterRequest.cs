namespace CRM_Backend.Models.DTOs.Auth;

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Hospital { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}