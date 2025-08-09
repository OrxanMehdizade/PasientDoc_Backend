namespace CRM_Backend.Models.Entities;

public class TreatmentForm : BaseEntity
{
    public string Name { get; set; } = null!;
    public string DoctorId { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual List<TreatmentFormContent>? TreatmentFormContent { get; set; } = [];
}
