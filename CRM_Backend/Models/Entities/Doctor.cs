using Microsoft.AspNetCore.Identity;

namespace CRM_Backend.Models.Entities;

public class Doctor : IdentityUser
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? WorkPhone { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? Specialization { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public TimeOnly? AppointmentDuration { get; set; }
    public string? OTP { get; set; }
    public string? Hospital { get; set; }
    public virtual List<Patient> Patients { get; set; } = [];
    public virtual List<Diagnose>? Diagnoses { get; set; }
    public virtual List<Medicine>? Medicines { get; set; }
    public virtual List<Service>? Services { get; set; }
    public virtual List<TreatmentForm>? TreatmentForms { get; set; }
    public virtual List<TreatmentFormContent>? TreatmentFormContents { get; set; }
}
