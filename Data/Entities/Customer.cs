namespace RepairTrack.Data.Entities;

public class Customer 
{ 
    public int CustomerId { get; set; }
    public string FullName { get; set; } = "";
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public List<Device> Devices { get; set; } = new();
}
