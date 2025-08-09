namespace CRM_Backend.Models.Entities;

public class Service : BaseEntity
{
    public string Name { get; set; } = null!;
    public string DoctorId { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual List<Appointment>? Appointments { get; set; }
    public virtual List<Treatment>? Treatments { get; set; }
}
