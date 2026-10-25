using Microsoft.EntityFrameworkCore;
using RepairTrack.Data.Entities;

namespace RepairTrack.Data.Repositories;

public class JobOrderRepository
{
    public JobOrder? GetById(int jobId)
    {
        using var db = new RepairTrackContext();
        return db.JobOrders
            .Include(j => j.Device).ThenInclude(d => d.Customer)
            .Include(j => j.StatusHistory)
            .Include(j => j.Payments)
            .FirstOrDefault(j => j.JobId == jobId);
    }

    public List<JobOrder> GetAll(string? status = null)
    {
        using var db = new RepairTrackContext();
        var query = db.JobOrders
            .Include(j => j.Device).ThenInclude(d => d.Customer)
            .Include(j => j.Payments)
            .AsQueryable();
        if (!string.IsNullOrEmpty(status))
            query = query.Where(j => j.Status == status);
        return query.OrderByDescending(j => j.DateReceived).ToList();
    }

    public List<JobOrder> GetByDevice(int deviceId)
    {
        using var db = new RepairTrackContext();
        return db.JobOrders.Include(j => j.Payments)
            .Where(j => j.DeviceId == deviceId).OrderByDescending(j => j.DateReceived).ToList();
    }

    public List<JobOrder> GetRecentActive(int count)
    {
        using var db = new RepairTrackContext();
        return db.JobOrders
            .Include(j => j.Device).ThenInclude(d => d.Customer)
            .Include(j => j.Payments)
            .Where(j => j.Status != "Completed")
            .OrderByDescending(j => j.DateReceived).Take(count).ToList();
    }

    public int CountByStatus(string status)
    {
        using var db = new RepairTrackContext();
        return db.JobOrders.Count(j => j.Status == status);
    }

    public int CountCompletedSince(DateTime from)
    {
        using var db = new RepairTrackContext();
        return db.JobOrders.Count(j => j.Status == "Completed" && j.DateReleased >= from);
    }

    public void Add(JobOrder job)
    {
        using var db = new RepairTrackContext();
        db.JobOrders.Add(job);
        db.SaveChanges();
    }

    public void Update(JobOrder job, JobStatusHistory? newHistory = null)
    {
        using var db = new RepairTrackContext();
        var existing = db.JobOrders.Find(job.JobId)
            ?? throw new InvalidOperationException("Job order not found.");
        db.Entry(existing).CurrentValues.SetValues(job);
        if (newHistory != null)
        {
            newHistory.JobId = job.JobId;
            db.JobStatusHistories.Add(newHistory);
        }
        db.SaveChanges();
    }
}