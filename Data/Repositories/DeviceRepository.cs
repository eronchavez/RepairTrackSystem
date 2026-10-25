using Microsoft.EntityFrameworkCore;
using RepairTrack.Data.Entities;

namespace RepairTrack.Data.Repositories;

public class DeviceRepository
{
    public List<Device> GetByCustomer(int customerId)
    {
        using var db = new RepairTrackContext();
        return db.Devices.Include(d => d.JobOrders)
            .Where(d => d.CustomerId == customerId).OrderBy(d => d.DeviceId).ToList();
    }

    public Device? GetById(int deviceId)
    {
        using var db = new RepairTrackContext();
        return db.Devices.Include(d => d.Customer).Include(d => d.JobOrders)
            .FirstOrDefault(d => d.DeviceId == deviceId);
    }

    public List<Device> SearchBySerial(string text)
    {
        using var db = new RepairTrackContext();
        string pattern = $"%{text}%";
        return db.Devices.Include(d => d.Customer)
            .Where(d => EF.Functions.ILike(d.SerialNumber, pattern))
            .OrderBy(d => d.SerialNumber).Take(20).ToList();
    }

    public void Add(Device device)
    {
        using var db = new RepairTrackContext();
        db.Devices.Add(device);
        db.SaveChanges();
    }

    public void Update(Device device)
    {
        using var db = new RepairTrackContext();
        var existing = db.Devices.Find(device.DeviceId)
            ?? throw new InvalidOperationException("Device not found.");
        db.Entry(existing).CurrentValues.SetValues(device);
        db.SaveChanges();
    }
}