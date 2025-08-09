using CRM_Backend.Models.DTOs.TreatmentForm;
using CRM_Backend.Models.Entities;

namespace CRM_Backend.Services.TreatmentFormServices;

public interface ITreatmentFormService
{
    Task<List<GetTreatFormsDto>> GetTreatmentFormsAsync();
    Task<TreatmentForm> GetTreatmentFormByIdAsync(int id);
    Task<int> CreateTreatmentFormAsync(CreateTreatmentFormRequest request);
    Task<bool> UpdateTreatmentFormAsync(int id,UpdateTreatmentFormRequest treatmentForm);
    Task<bool> DeleteTreatmentFormAsync(int id);
}
