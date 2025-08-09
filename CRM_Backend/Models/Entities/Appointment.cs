using CRM_Backend.Models.Enums;

namespace CRM_Backend.Models.Entities;

public class Appointment : BaseEntity
{
    public string? Hospital { get; set; }
    public DateTime Date { get; set; }
    public Appeal Appeal { get; set; }
    public bool IsFinished { get; set; }
    public bool IsCancelled { get; set; }
    public bool IsPending { get; set; } = true;
    public int PatientId { get; set; }
    public virtual Patient Patient { get; set; } = null!;
    public string DoctorId { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public int? ServiceId { get; set; }
    public virtual Service? Service { get; set; }
}
