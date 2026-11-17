using RepairTrack.Data.Entities;
using RepairTrack.Data.Repositories;

namespace RepairTrack.Services;

public class CustomerService
{
    private readonly CustomerRepository _customers = new();

    public List<Customer> GetAll()
    {
        return _customers.GetAll();
    }

    public Customer? GetById(int customerId)
    {
        return _customers.GetById(customerId);
    }

    public int DeviceCount(Customer customer)
    {
        return customer.Devices.Count;
    }

    public List<Customer> Search(string text)
    {
        return _customers.Search(text);
    }

    public SaveResult Add(string fullName, string? phone, string? address)
    {
        string? error = ValidateName(fullName);
        if (error != null)
            return SaveResult.Fail(error);

        var customer = new Customer
        {
            FullName = fullName.Trim(),
            Phone = EmptyToNull(phone),
            Address = EmptyToNull(address)
        };
        try
        {
            _customers.Add(customer);
            return SaveResult.Success(customer.CustomerId);
        }
        catch (Exception)
        {
            return SaveResult.Fail("Could not save the customer. Please try again.");
        }
    }

    public SaveResult Update(int customerId, string fullName, string? phone, string? address)
    {
        string? error = ValidateName(fullName);
        if (error != null)
            return SaveResult.Fail(error);

        var customer = _customers.GetById(customerId);
        if (customer == null)
            return SaveResult.Fail("Customer not found.");

        customer.FullName = fullName.Trim();
        customer.Phone = EmptyToNull(phone);
        customer.Address = EmptyToNull(address);
        try
        {
            _customers.Update(customer);
            return SaveResult.Success(customerId);
        }
        catch (Exception)
        {
            return SaveResult.Fail("Could not save the customer. Please try again.");
        }
    }

    private static string? ValidateName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return "Please enter the customer's full name.";
        if (fullName.Trim().Length > 100)
            return "Name is too long (maximum 100 characters).";
        return null;
    }

    private static string? EmptyToNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        return value.Trim();
    }
}
