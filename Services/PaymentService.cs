using RepairTrack.Data.Entities;
using RepairTrack.Data.Repositories;

namespace RepairTrack.Services;

public class PaymentService
{
    private readonly PaymentRepository _payments = new();
    private readonly JobOrderRepository _jobs = new();
    private readonly JobOrderService _jobService = new();

    public List<Payment> GetByJob(int jobId)
    {
        return _payments.GetByJob(jobId);
    }

    public SaveResult Record(int jobId, decimal amount, DateOnly paymentDate)
    {
        if (amount <= 0)
            return SaveResult.Fail("Payment amount must be greater than zero.");

        var job = _jobs.GetById(jobId);
        if (job == null)
            return SaveResult.Fail("Job order not found.");

        decimal balance = _jobService.GetBalance(job);
        if (amount > balance)
            return SaveResult.Fail("Payment cannot be more than the remaining balance (" + FormatMoney(balance) + ").");

        try
        {
            _payments.Add(new Payment
            {
                JobId = jobId,
                AmountPaid = amount,
                PaymentDate = paymentDate
            });
            return SaveResult.Success();
        }
        catch (Exception)
        {
            return SaveResult.Fail("Could not record the payment. Please try again.");
        }
    }

    private static string FormatMoney(decimal amount)
    {
        return "₱" + amount.ToString("#,##0.00");
    }
}
