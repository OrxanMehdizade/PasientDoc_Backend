namespace CRM_Backend.Models.Entities;

public class Medicine : BaseEntity
{
    public string Name { get; set; } = null!;
    public string DoctorId { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual List<MedicineTreatment> MedicineTreatments { get; set; } = [];
}