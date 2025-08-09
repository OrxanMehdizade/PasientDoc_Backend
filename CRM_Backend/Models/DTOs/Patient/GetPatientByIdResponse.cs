using CRM_Backend.Models.DTOs.Appointment;
using CRM_Backend.Models.DTOs.Treatment;

namespace CRM_Backend.Models.DTOs.Patient
{
    public class GetPatientByIdResponse
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FatherName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Insurance { get; set; }
        public string DoctorReferral { get; set; }
        public List<AppointmentDto> Appointments { get; set; }
        public List<TreatmentDto> Treatments { get; set; }
    }
}
