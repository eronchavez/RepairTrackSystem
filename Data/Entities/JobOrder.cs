namespace RepairTrack.Data.Entities;

public class JobOrder
{
    public int JobId { get; set; }
    public int DeviceId { get; set; }
    public string IssueDescription { get; set; } = "";
    public string Status { get; set; } = "Received";
    public DateTime DateReceived { get; set; }
    public DateTime? DateReleased { get; set; }
    public decimal TotalCost { get; set; }
    public Device Device { get; set; } = null!;
    public List<JobStatusHistory> StatusHistory { get; set; } = new();
    public List<Payment> Payments { get; set; } = new();
}