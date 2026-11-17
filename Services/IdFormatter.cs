namespace RepairTrack.Services;

public static class IdFormatter
{
    public static string JobId(int jobId, DateTime dateReceived)
    {
        return $"JO-{dateReceived.Year}-{jobId:D4}";
    }

    public static string CustomerId(int customerId)
    {
        return $"C-{customerId:D5}";
    }
}
