namespace CRM_Backend.Models.Entities;

public class Diagnose : BaseEntity
{
    public string Name { get; set; } = null!;
    public string DoctorId { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual List<DiagnoseTreatment> DiagnoseTreatments { get; set; } = [];

}
