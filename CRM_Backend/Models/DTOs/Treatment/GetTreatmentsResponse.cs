namespace CRM_Backend.Models.DTOs.Treatment;

public class GetTreatmentsResponse
{
    public int Id { get; set; }
    public string PatientName { get; set; } = null!;
    public DateTime? Date { get; set; }
    public string? ServiceName { get; set; }
    public List<string> DiagnoseNames { get; set; } = [];
}
