namespace CRM_Backend.Models.DTOs.Auth;

public class VerifyOtpRequest
{
    public string Email { get; set; }
    public string VerificationCode { get; set; }
}
