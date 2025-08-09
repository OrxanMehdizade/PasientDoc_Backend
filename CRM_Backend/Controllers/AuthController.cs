using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRM_Backend.Auth;
using CRM_Backend.Models.Entities;
using CRM_Backend.Models.DTOs.Auth;
using CRM_Backend.Services.VerificationServices;
using Microsoft.AspNetCore.Authorization;
using CRM_Backend.Providers;

namespace CRM_Backend.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class AuthController(UserManager<Doctor> userManager, SignInManager<Doctor> signInManager, IJwtService jwtService, IVerificationCodeService verificationCodeService, IDoctorProvider doctorProvider) : ControllerBase
{
    private readonly IDoctorProvider _doctorProvider = doctorProvider;
    private readonly UserManager<Doctor> _userManager = userManager;
    private readonly SignInManager<Doctor> _signInManager = signInManager;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IVerificationCodeService _verificationCodeService = verificationCodeService;

    private async Task<AuthTokenDto> GenerateToken(Doctor user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);

        var accessToken = _jwtService.GenerateSecurityToken(user.Id, user.Name!, roles, claims);

        var refreshToken = Guid.NewGuid().ToString().ToLower();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(3);
        await _userManager.UpdateAsync(user);

        return new AuthTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            FullName = $"{user.Name} {user.Surname}",
            Email = user.Email!,
            ProfileImgUrl = user.ProfileImageUrl!
        };
    }

    [HttpPost("check-email")]
    public async Task<IActionResult> CheckEmail(CheckEmailRequest request)
    {
        var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null)
            return Conflict("User already exists");
        var verificationCode = _verificationCodeService.GenerateCode(4);
        var newUser = new Doctor
        {
            Email = request.Email,
            UserName = request.Email,
            OTP = verificationCode
        };
        var dbresult = await _userManager.CreateAsync(newUser);
        if (!dbresult.Succeeded)
        {
            return BadRequest(dbresult.Errors);
        }

        var message = $"Your OTP code is {verificationCode}";
        var result = await _verificationCodeService.SendVerificationCodeAsync(request.Email, message);
        if (result != "Ok") BadRequest(result);

        var response = new
        {
            message = "Verification code sent",
            email = request.Email
        };

        return Ok(response);
    }

    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp(CheckEmailRequest request)
    {
        var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser!.Name != null)
            return Conflict("User already exists");
        var verificationCode = _verificationCodeService.GenerateCode(4);
        existingUser.OTP = verificationCode;
        var dbresult = await _userManager.UpdateAsync(existingUser);
        if (!dbresult.Succeeded)
        {
            return BadRequest(dbresult.Errors);
        }

        var message = $"Your OTP code is {verificationCode}";
        var result = await _verificationCodeService.SendVerificationCodeAsync(request.Email, message);
        if (result != "Ok") BadRequest(result);

        var response = new
        {
            message = "Verification code sent",
            email = request.Email
        };

        return Ok(response);
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(VerifyOtpRequest request)
    {
        var isValid = await _verificationCodeService.ValidateVerificationCode(request.Email, request.VerificationCode);
        if (!isValid)
            return BadRequest("Invalid verification code");

        return Ok("OTP verified");
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthTokenDto>> Register(RegisterRequest request)
    {
        var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null) await _userManager.DeleteAsync(existingUser);
        else return BadRequest("User does not exist!");

        var user = new Doctor
        {
            Email = request.Email,
            Name = request.Name,
            Surname = request.Surname,
            UserName = request.Email,
            Hospital = request.Hospital,
            ProfileImageUrl = $"https://ui-avatars.com/api/?name={request.Name}%20{request.Surname}&background=random&rounded=true"
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return await GenerateToken(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthTokenDto>> Login(LoginRequest request)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
            return BadRequest("Email or password is not correct!");

        var canSignIn = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!canSignIn.Succeeded)
            return BadRequest("Email or password is not correct!");

        return await GenerateToken(user);
    }

    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest request)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
            return BadRequest("User not found!");

        var newPassword = _verificationCodeService.GenerateCode(6);
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetResult = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!resetResult.Succeeded)
            return BadRequest(resetResult.Errors);

        var result = await _verificationCodeService.SendVerificationCodeAsync(request.Email, $"Your new password is: {newPassword}");
        if (result != "Ok") BadRequest(result);

        return Ok("New password sent to your email.");
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var user = await _userManager.FindByIdAsync(doctorId!);
        if (user is null)
            return BadRequest();

        user.RefreshToken = "";
        user.RefreshTokenExpiryTime = DateTime.MinValue;
        await _userManager.UpdateAsync(user);
        return Ok("successfull logout");
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthTokenDto>> RefreshToken(RefreshRequest request)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);
        if (user == null) return Unauthorized();
        if (user.RefreshTokenExpiryTime < DateTime.Now)
        {
            user.RefreshToken = "";
            user.RefreshTokenExpiryTime = DateTime.MinValue;
            return Unauthorized();
        }

        return await GenerateToken(user);
    }
}
