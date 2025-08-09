using CRM_Backend.Models.DTOs.Pagination;
using CRM_Backend.Models.DTOs.Treatment;
using CRM_Backend.Models.Entities;

namespace CRM_Backend.Services.TreatmentServices;

public interface ITreatmentService
{
    Task<PaginatedListDto<GetTreatmentsResponse>> GetTreatmentsAsync(PaginationRequest paginationRequest);
    Task<GetTreatmentByIdResponse> GetTreatmentByIdAsync(int id);
    Task<bool> CreateTreatmentAsync(CreateTreatmentRequest request);
    Task<bool> UpdateTreatmentAsync(int id, UpdateTreatmentRequest request);
    Task<bool> DeleteTreatmentAsync(int id);
}
