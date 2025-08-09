using CRM_Backend.Models.DTOs.Pagination;
using CRM_Backend.Models.DTOs.Patient;
using CRM_Backend.Models.Entities;

namespace CRM_Backend.Services.PatientServices;

public interface IPatientService
{
    Task<PaginatedListDto<GetAllPatientsResponse>> GetPatientsAsync(PaginationRequest paginationRequest);
    Task<List<GetAllSearchPatientsResponse>> GetSearchPatientsAsync();
    Task<GetPatientByIdResponse> GetPatientByIdAsync(int id);
    Task<Patient> CreatePatientAsync(CreatePatientDto patientDto);
    Task<bool> UpdatePatientAsync(int id,UpdatePatientRequest request);
    Task<bool> DeletePatientAsync(int id);
}
