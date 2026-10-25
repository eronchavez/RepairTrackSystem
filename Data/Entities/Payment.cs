namespace RepairTrack.Data.Entities;

public class Payment
{
    public int PaymentId { get; set; }
    public int JobId { get; set; }
    public decimal AmountPaid { get; set; }
    public DateOnly PaymentDate { get; set; }
    public JobOrder JobOrder { get; set; } = null!;
}