using Microsoft.EntityFrameworkCore;
using RepairTrack.Data.Entities;

namespace RepairTrack.Data.Repositories;

public class CustomerRepository
{
    public List<Customer> GetAll()
    {
        using var db = new RepairTrackContext();
        return db.Customers.Include(c => c.Devices).OrderBy(c => c.FullName).ToList();
    }

    public Customer? GetById(int customerId)
    {
        using var db = new RepairTrackContext();
        return db.Customers.Include(c => c.Devices).FirstOrDefault(c => c.CustomerId == customerId);
    }

    public List<Customer> Search(string text)
    {
        using var db = new RepairTrackContext();
        string pattern = $"%{text}%";
        return db.Customers
            .Where(c => EF.Functions.ILike(c.FullName, pattern)
                     || (c.Phone != null && EF.Functions.ILike(c.Phone, pattern)))
            .OrderBy(c => c.FullName).Take(20).ToList();
    }

    public void Add(Customer customer)
    {
        using var db = new RepairTrackContext();
        db.Customers.Add(customer);
        db.SaveChanges();
    }

    public void Update(Customer customer)
    {
        using var db = new RepairTrackContext();
        var existing = db.Customers.Find(customer.CustomerId)
            ?? throw new InvalidOperationException("Customer not found.");
        db.Entry(existing).CurrentValues.SetValues(customer);
        db.SaveChanges();
    }
}