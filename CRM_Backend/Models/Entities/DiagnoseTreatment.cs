namespace CRM_Backend.Models.Entities;

public class DiagnoseTreatment
{
    public int DiagnoseId { get; set; }
    public Diagnose Diagnose { get; set; } = null!;

    public int TreatmentId { get; set; }
    public Treatment Treatment { get; set; } = null!;
}
