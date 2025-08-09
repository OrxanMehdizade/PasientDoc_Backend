using CRM_Backend.Models.DTOs.Service;
using CRM_Backend.Models.Entities;

namespace CRM_Backend.Services.ServiceServices;

public interface IServiceService
{
    Task<List<GetServicesResponse>> GetServicesAsync();
    Task<int> CreateServiceAsync(CreateServiceRequest request);
    Task<bool> UpdateServiceAsync(int id, UpdateServiceRequest request);
    Task<bool> DeleteServiceAsync(int id);
}
