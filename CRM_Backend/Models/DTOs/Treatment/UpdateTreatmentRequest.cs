using CRM_Backend.Models.DTOs.TreatmetnFormContent;

namespace CRM_Backend.Models.DTOs.Treatment;

public class UpdateTreatmentRequest
{
    public int PatientId { get; set; }
    public int ServiceId { get; set; }
    public List<int> DiagnoseIds { get; set; } = new();
    public List<int> MedicineIds { get; set; } = new();
    public List<TreatmentFormContentDto> TreatmentForms { get; set; } = new();
    public string? DocumentUrl { get; set; }
}