namespace CRM_Backend.Models.Entities;

public class TreatmentFormContent : BaseEntity
{
    public string? Content { get; set; }
    public string DoctorId { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public int TreatmentId { get; set; }
    public virtual Treatment Treatment { get; set; } = null!;
    public int TreatmentFormId { get; set; }
    public virtual TreatmentForm TreatmentForm { get; set; } = null!;
}
