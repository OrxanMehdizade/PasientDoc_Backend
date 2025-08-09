namespace CRM_Backend.Models.DTOs.Patient;

public class GetAllPatientsResponse
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Gender { get; set; }
    public int Age { get; set; }
}

