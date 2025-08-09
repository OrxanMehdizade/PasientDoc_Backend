namespace CRM_Backend.Services.VerificationServices;

public interface IVerificationCodeService
{
    Task<string> SendVerificationCodeAsync(string email, string code);
    string GenerateCode(int length);
    Task<bool> ValidateVerificationCode(string email, string code);
}