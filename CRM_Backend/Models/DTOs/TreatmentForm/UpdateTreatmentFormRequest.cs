namespace CRM_Backend.Models.DTOs.TreatmentForm;

public class UpdateTreatmentFormRequest
{
    public string Name { get; set; } = null!;
    public string? Content { get; set; }
}
