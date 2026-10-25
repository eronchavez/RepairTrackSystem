namespace RepairTrack.Data.Entities;

public class JobStatusHistory
{
    public int HistoryId { get; set; }
    public int JobId { get; set; }
    public string Status { get; set; } = "";
    public DateTime ChangedAt { get; set; }
    public JobOrder JobOrder { get; set; } = null!;
}