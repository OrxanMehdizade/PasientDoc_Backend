namespace CRM_Backend.Models.Entities;

public class MedicineTreatment
{
    public int MedicineId { get; set; }
    public Medicine Medicine { get; set; } = null!;

    public int TreatmentId { get; set; }
    public Treatment Treatment { get; set; } = null!;
}
