using CRM_Backend.Models.DTOs.Profile;
using CRM_Backend.Services.ProfileServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProfileController(IProfileService profileService) : ControllerBase
{
    private readonly IProfileService _profileService = profileService;

    [HttpGet("get-profile-data")]
    public async Task<ActionResult<GetProfileDataResponse>> GetProfileData()
    {
        try
        {
            var profile = await _profileService.GetProfileDataAsync();

            return Ok(profile);
        }
        catch (Exception ex)
        {

            return BadRequest(ex.Message);
        }
    }

    [HttpPut("update-profile-data")]
    public async Task<IActionResult> UpdateProfileData(UpdateProfileRequest request)
    {
        try
        {
            var success = await _profileService.UpdateProfileDataAsync(request);

            if (!success) throw new Exception("Update failed");

            return Ok("Profile updated successfully");

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        try
        {
            var success = await _profileService.ChangePasswordAsync(request);

            if (!success) throw new Exception("Change password failed");

            return Ok("Successfully changed");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
