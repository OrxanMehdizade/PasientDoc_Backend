using CRM_Backend.Models.DTOs.Diagnose;
using CRM_Backend.Models.DTOs.Medicine;
using CRM_Backend.Models.DTOs.TreatmentForm;

namespace CRM_Backend.Models.DTOs.Treatment;

public class GetTreatmentByIdResponse
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = null!;
    public int? ServiceId { get; set; }
    public string? ServiceName { get; set; }
    public List<GetDiagnosesResponse> Diagnoses { get; set; } = new();
    public List<GetMedicinesResponse> Medicines { get; set; } = new();
    public List<TreatmentFormDto> TreatmentForms { get; set; } = new();
    public string? DocumentUrl { get; set; }
}
