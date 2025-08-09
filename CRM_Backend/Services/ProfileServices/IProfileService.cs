using CRM_Backend.Models.DTOs.Profile;

namespace CRM_Backend.Services.ProfileServices;

public interface IProfileService
{
    Task<GetProfileDataResponse> GetProfileDataAsync();
    Task<bool> UpdateProfileDataAsync(UpdateProfileRequest request);
    Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
}
