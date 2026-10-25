using RepairTrack.Data.Entities;

namespace RepairTrack.Data.Repositories;

public class PaymentRepository
{
    public List<Payment> GetByJob(int jobId)
    {
        using var db = new RepairTrackContext();
        return db.Payments.Where(p => p.JobId == jobId)
            .OrderBy(p => p.PaymentDate).ThenBy(p => p.PaymentId).ToList();
    }

    public decimal GetTotalPaid(int jobId)
    {
        using var db = new RepairTrackContext();
        return db.Payments.Where(p => p.JobId == jobId).Sum(p => p.AmountPaid);
    }

    public void Add(Payment payment)
    {
        using var db = new RepairTrackContext();
        db.Payments.Add(payment);
        db.SaveChanges();
    }
}