namespace RepairTrack.Data.Entities;

public class Device
{
    public int DeviceId { get; set; }
    public int CustomerId { get; set; }
    public string DeviceType { get; set; } = "";
    public string? Brand { get; set; }
    public string SerialNumber { get; set; } = "";
    public string? Password { get; set; }
    public Customer Customer { get; set; } = null!;
    public List<JobOrder> JobOrders { get; set; } = new();
}