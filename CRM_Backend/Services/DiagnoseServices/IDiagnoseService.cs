using CRM_Backend.Models.DTOs.Diagnose;
using CRM_Backend.Models.Entities;

namespace CRM_Backend.Services.DiagnoseServices;

public interface IDiagnoseService
{
    Task<List<GetDiagnosesResponse>> GetDiagnosesAsync();
    Task<int> CreateDiagnoseAsync(CreateDiagnoseRequest request);
    Task<bool> UpdateDiagnoseAsync(int id,UpdateDiagnoseRequest request);
    Task<bool> DeleteDiagnoseAsync(int id);
}
