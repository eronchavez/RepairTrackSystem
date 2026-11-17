using RepairTrack.Data.Entities;
using RepairTrack.Data.Repositories;

namespace RepairTrack.Services;

public class DashboardService
{
    private readonly JobOrderRepository _jobs = new();

    public int CountReceived()
    {
        return _jobs.CountByStatus("Received");
    }

    public int CountInRepair()
    {
        return _jobs.CountByStatus("In Repair");
    }

    public int CountReadyForPickup()
    {
        return _jobs.CountByStatus("Ready for Pickup");
    }

    public int CountCompletedThisMonth()
    {
        DateTime today = DateTime.Now;
        DateTime monthStart = new DateTime(today.Year, today.Month, 1);
        return _jobs.CountCompletedSince(monthStart);
    }

    public List<JobOrder> GetRecentActive()
    {
        return _jobs.GetRecentActive(10);
    }
}
