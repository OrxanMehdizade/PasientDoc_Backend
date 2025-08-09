using CRM_Backend.Models.Enums;

namespace CRM_Backend.Models.Entities;

public class Patient : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string FatherName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime? BirthDate { get; set; }
    public Gender Gender { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? Insurance { get; set; }
    public string? DoctorReferral { get; set; }
    public string DoctorId { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public List<Appointment>? Appointments { get; set; }
    public List<Treatment>? Treatments { get; set; }
}