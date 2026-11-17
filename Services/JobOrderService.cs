using RepairTrack.Data.Entities;
using RepairTrack.Data.Repositories;

namespace RepairTrack.Services;

public class JobOrderService
{
    public static readonly string[] StatusSteps =
    {
        "Received", "In Repair", "Ready for Pickup", "Completed"
    };

    private readonly JobOrderRepository _jobs = new();

    public List<JobOrder> GetAll(string? status = null)
    {
        if (status == "All")
            status = null;
        return _jobs.GetAll(status);
    }

    public JobOrder? GetById(int jobId)
    {
        return _jobs.GetById(jobId);
    }

    public List<JobOrder> GetByDevice(int deviceId)
    {
        return _jobs.GetByDevice(deviceId);
    }

    public List<JobOrder> GetByCustomer(int customerId)
    {
        return _jobs.GetByCustomer(customerId);
    }

    public string? GetNextStatus(string current)
    {
        int i = Array.IndexOf(StatusSteps, current);
        if (i < 0 || i >= StatusSteps.Length - 1)
            return null;
        return StatusSteps[i + 1];
    }

    public string? GetPreviousStatus(string current)
    {
        int i = Array.IndexOf(StatusSteps, current);
        if (i <= 0)
            return null;
        return StatusSteps[i - 1];
    }

    public decimal GetBalance(JobOrder job)
    {
        decimal paid = 0;
        foreach (var payment in job.Payments)
            paid += payment.AmountPaid;
        return job.TotalCost - paid;
    }

    public decimal GetTotalPaid(JobOrder job)
    {
        return job.TotalCost - GetBalance(job);
    }

    public SaveResult Create(int deviceId, string issueDescription, decimal totalCost)
    {
        if (string.IsNullOrWhiteSpace(issueDescription))
            return SaveResult.Fail("Please describe the issue with the device.");
        if (totalCost < 0)
            return SaveResult.Fail("Total cost cannot be negative.");

        DateTime now = UnspecifiedNow();
        var job = new JobOrder
        {
            DeviceId = deviceId,
            IssueDescription = issueDescription.Trim(),
            Status = "Received",
            DateReceived = now,
            TotalCost = totalCost
        };
        job.StatusHistory.Add(new JobStatusHistory
        {
            Status = "Received",
            ChangedAt = now
        });

        try
        {
            _jobs.Add(job);
            return SaveResult.Success(job.JobId);
        }
        catch (Exception)
        {
            return SaveResult.Fail("Could not save the job order. Please try again.");
        }
    }

    public SaveResult UpdateCost(int jobId, decimal totalCost)
    {
        if (totalCost < 0)
            return SaveResult.Fail("Total cost cannot be negative.");

        var job = _jobs.GetById(jobId);
        if (job == null)
            return SaveResult.Fail("Job order not found.");

        decimal paid = GetTotalPaid(job);
        if (totalCost < paid)
            return SaveResult.Fail("Total cost cannot be less than the amount already paid.");

        job.TotalCost = totalCost;
        try
        {
            _jobs.Update(job);
            return SaveResult.Success(jobId);
        }
        catch (Exception)
        {
            return SaveResult.Fail("Could not update the total cost. Please try again.");
        }
    }

    public SaveResult UpdateIssue(int jobId, string issueDescription)
    {
        if (string.IsNullOrWhiteSpace(issueDescription))
            return SaveResult.Fail("Please describe the issue with the device.");

        var job = _jobs.GetById(jobId);
        if (job == null)
            return SaveResult.Fail("Job order not found.");

        job.IssueDescription = issueDescription.Trim();
        try
        {
            _jobs.Update(job);
            return SaveResult.Success(jobId);
        }
        catch (Exception)
        {
            return SaveResult.Fail("Could not save the issue description. Please try again.");
        }
    }

    public SaveResult ChangeStatus(int jobId, string newStatus)
    {
        var job = _jobs.GetById(jobId);
        if (job == null)
            return SaveResult.Fail("Job order not found.");

        int from = Array.IndexOf(StatusSteps, job.Status);
        int to = Array.IndexOf(StatusSteps, newStatus);
        if (from < 0 || to < 0)
            return SaveResult.Fail("Unknown status.");
        if (from == to)
            return SaveResult.Success(jobId);
        if (Math.Abs(to - from) != 1)
            return SaveResult.Fail("You can only move one step forward or one step back.");

        job.Status = newStatus;
        if (newStatus == "Completed")
            job.DateReleased = UnspecifiedNow();
        else
            job.DateReleased = null;

        var history = new JobStatusHistory
        {
            Status = newStatus,
            ChangedAt = UnspecifiedNow()
        };

        try
        {
            _jobs.Update(job, history);
            return SaveResult.Success(jobId);
        }
        catch (Exception)
        {
            return SaveResult.Fail("Could not update the status. Please try again.");
        }
    }

    private static DateTime UnspecifiedNow()
    {
        return DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
    }
}
