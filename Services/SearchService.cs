using RepairTrack.Data.Repositories;

namespace RepairTrack.Services;

public class SearchHit
{
    public string Kind { get; init; } = "";
    public int Id { get; init; }
    public string Title { get; init; } = "";
    public string Subtitle { get; init; } = "";

    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(Subtitle))
            return Kind + "  " + Title;
        return Kind + "  " + Title + "  ·  " + Subtitle;
    }
}

public class SearchService
{
    private readonly CustomerRepository _customers = new();
    private readonly DeviceRepository _devices = new();

    public List<SearchHit> Search(string text)
    {
        var hits = new List<SearchHit>();
        if (string.IsNullOrWhiteSpace(text))
            return hits;

        string q = text.Trim();

        foreach (var customer in _customers.Search(q))
        {
            hits.Add(new SearchHit
            {
                Kind = "Customer",
                Id = customer.CustomerId,
                Title = customer.FullName,
                Subtitle = string.IsNullOrWhiteSpace(customer.Phone)
                    ? IdFormatter.CustomerId(customer.CustomerId)
                    : customer.Phone + "  " + IdFormatter.CustomerId(customer.CustomerId)
            });
        }

        foreach (var device in _devices.SearchBySerial(q))
        {
            string title = device.DeviceType;
            if (!string.IsNullOrWhiteSpace(device.Brand))
                title = device.Brand + " " + device.DeviceType;

            hits.Add(new SearchHit
            {
                Kind = "Device",
                Id = device.DeviceId,
                Title = title,
                Subtitle = device.SerialNumber + "  ·  " + device.Customer.FullName
            });
        }

        return hits;
    }
}
