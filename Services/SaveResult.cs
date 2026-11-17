namespace RepairTrack.Services;

public class SaveResult
{
    public bool Ok { get; init; }
    public string Message { get; init; } = "";
    public int Id { get; init; }

    public static SaveResult Fail(string message)
    {
        return new SaveResult { Ok = false, Message = message };
    }

    public static SaveResult Success(int id = 0)
    {
        return new SaveResult { Ok = true, Id = id };
    }
}
