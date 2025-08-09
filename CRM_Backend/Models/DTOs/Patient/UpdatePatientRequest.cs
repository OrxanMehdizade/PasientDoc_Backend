using CRM_Backend.Models.Enums;

namespace CRM_Backend.Models.DTOs.Patient;

public class UpdatePatientRequest
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string FatherName { get; set; }
    public string Phone { get; set; }
    public DateTime BirthDay { get; set; }
    public Gender Gender { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string Insurance { get; set; }
    public string DoctorReferral { get; set; }
}
