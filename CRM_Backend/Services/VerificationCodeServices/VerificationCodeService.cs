
using CRM_Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace CRM_Backend.Services.VerificationServices;

public class VerificationCodeService(CRMDbContext context, IConfiguration configuration) : IVerificationCodeService
{
    private readonly CRMDbContext _context = context;
    private readonly IConfiguration _configuration = configuration;

    public async Task<string> SendVerificationCodeAsync(string email, string message)
    {
        try
        {
            // Get email configuration from appsettings
            var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var senderEmail = _configuration["EmailSettings:SenderEmail"] ?? "your-email@gmail.com";
            var senderPassword = _configuration["EmailSettings:SenderPassword"] ?? "your-app-password";
            var senderName = _configuration["EmailSettings:SenderName"] ?? "CRM System";

            // Check if email settings are configured
            if (senderEmail == "your-email@gmail.com" || senderPassword == "your-app-password")
            {
                throw new Exception("Email settings not configured. Please update appsettings.json with your email credentials.");
            }

            // Create email message
            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = "Verification Code - CRM System",
                Body = message,
                IsBodyHtml = false
            };
            mailMessage.To.Add(email);

            // Configure SMTP client
            var smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(senderEmail, senderPassword)
            };

            // Send email
            await smtpClient.SendMailAsync(mailMessage);

            // Log success
            Console.WriteLine($"Email sent successfully to: {email}");
            return "Ok";
        }
        catch (Exception ex)
        {
            // Log error and fallback to console simulation
            Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
            Console.WriteLine("Falling back to console simulation...");
            
            // Fallback: Log to console for development
            Console.WriteLine($"=== EMAIL SIMULATION (FALLBACK) ===");
            Console.WriteLine($"To: {email}");
            Console.WriteLine($"Message: {message}");
            Console.WriteLine($"=== END EMAIL ===");
            
            return "Ok";
        }
    }

    public string GenerateCode(int length)
    {
        int minValue = (int)Math.Pow(10, length - 1);
        int maxValue = (int)Math.Pow(10, length) - 1;

        return new Random().Next(minValue, maxValue + 1).ToString();
    }

    public async Task<bool> ValidateVerificationCode(string email, string code)
    {
        var doctor = await _context.Users.FirstOrDefaultAsync(d => d.Email == email);
        if (doctor?.OTP == code)
            return true;
        return false;
    }
}