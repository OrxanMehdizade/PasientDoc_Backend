using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CRM_Backend.Providers;

public class DoctorProvider(IHttpContextAccessor httpContextAccessor) : IDoctorProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string GetDoctorId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.Claims.FirstOrDefault(c => c.Type == "userId")?.Value!;
    }
}