using CRM_Backend.Models.DTOs.Profile;
using CRM_Backend.Models.Entities;
using CRM_Backend.Providers;
using Microsoft.AspNetCore.Identity;

namespace CRM_Backend.Services.ProfileServices;

public class ProfileService(UserManager<Doctor> userManager, IDoctorProvider doctorProvider) : IProfileService
{
    private readonly UserManager<Doctor> _userManager = userManager;
    private readonly IDoctorProvider _doctorProvider = doctorProvider;

    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var doctorId = _doctorProvider.GetDoctorId();

        var doctor = await _userManager.FindByIdAsync(doctorId);
        if (doctor is null) { throw new Exception("Doctor not found"); }
        var result = await _userManager.ChangePasswordAsync(doctor, request.CurrentPassword!, request.NewPassword!);

        return result.Succeeded;
    }

    public async Task<GetProfileDataResponse> GetProfileDataAsync()
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var doctor = await _userManager.FindByIdAsync(doctorId);
        return doctor == null
            ? throw new Exception("Doctor not found")
            : new GetProfileDataResponse
            {
                HospitalName = doctor.Hospital!,
                FullName = $"{doctor.Name} {doctor.Surname}",
                Specialization = doctor.Specialization!,
                Email = doctor.Email!,
                Phone = doctor.PhoneNumber!,
                WorkPhone = doctor.WorkPhone!,
                ProfileImageUrl = doctor.ProfileImageUrl!,
                AppointmentDuration = doctor.AppointmentDuration
            };
    }

    public async Task<bool> UpdateProfileDataAsync(UpdateProfileRequest request)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var doctor = await _userManager.FindByIdAsync(doctorId);
        if (doctor == null) throw new Exception("Doctor not found");

        if (!string.IsNullOrWhiteSpace(request.Email)) doctor.Email = request.Email;
        if (!string.IsNullOrWhiteSpace(request.WorkPhone)) doctor.WorkPhone = request.WorkPhone;
        if (!string.IsNullOrWhiteSpace(request.HospitalName)) doctor.Hospital = request.HospitalName;
        if (!string.IsNullOrWhiteSpace(request.Specialization)) doctor.Specialization = request.Specialization;
        if (!string.IsNullOrWhiteSpace(request.ProfilePictureImageUrl)) doctor.ProfileImageUrl = request.ProfilePictureImageUrl;

        var result = await _userManager.UpdateAsync(doctor);
        return result.Succeeded;
    }
}

