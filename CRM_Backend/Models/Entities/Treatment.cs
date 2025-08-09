namespace CRM_Backend.Models.Entities;

public class Treatment : BaseEntity
{
    public int PatientId { get; set; }
    public virtual Patient Patient { get; set; } = null!;
    public DateTime? Date { get; set; }
    public string? DocumentUrl { get; set; }
    public int? ServiceId { get; set; }
    public virtual Service? Service { get; set; }
    public string DoctorId { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual List<MedicineTreatment> MedicineTreatments { get; set; } = [];
    public virtual List<DiagnoseTreatment> DiagnoseTreatments { get; set; } = [];
    public virtual List<TreatmentFormContent>? TreatmentFormContents { get; set; } = [];
}
