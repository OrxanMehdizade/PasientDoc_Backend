using CRM_Backend.Models.DTOs.Medicine;

namespace CRM_Backend.Services.MedicineServices;
public interface IMedicineService
{
    Task<List<GetMedicinesResponse>> GetMedicinesAsync();
    Task<int> CreateMedicineAsync(CreateMedicineRequest request);
    Task<bool> UpdateMedicineAsync(int id, UpdateMedicineRequest request);
    Task<bool> DeleteMedicineAsync(int id);
}
