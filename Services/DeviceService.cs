using RepairTrack.Data.Entities;
using RepairTrack.Data.Repositories;

namespace RepairTrack.Services;

public class DeviceService
{
    private readonly DeviceRepository _devices = new();

    public List<Device> GetAll()
    {
        return _devices.GetAll();
    }

    public List<Device> GetByCustomer(int customerId)
    {
        return _devices.GetByCustomer(customerId);
    }

    public List<Device> SearchBySerial(string text)
    {
        return _devices.SearchBySerial(text);
    }

    public Device? GetById(int deviceId)
    {
        return _devices.GetById(deviceId);
    }

    public SaveResult Add(int customerId, string deviceType, string? brand, string serialNumber, string? password)
    {
        string? error = Validate(deviceType, serialNumber, brand, password);
        if (error != null)
            return SaveResult.Fail(error);

        var device = new Device
        {
            CustomerId = customerId,
            DeviceType = deviceType.Trim(),
            Brand = EmptyToNull(brand),
            SerialNumber = serialNumber.Trim(),
            Password = EmptyToNull(password)
        };
        try
        {
            _devices.Add(device);
            return SaveResult.Success(device.DeviceId);
        }
        catch (Exception)
        {
            return SaveResult.Fail("Could not save the device. Please try again.");
        }
    }

    public SaveResult Update(int deviceId, string deviceType, string? brand, string serialNumber, string? password)
    {
        string? error = Validate(deviceType, serialNumber, brand, password);
        if (error != null)
            return SaveResult.Fail(error);

        var device = _devices.GetById(deviceId);
        if (device == null)
            return SaveResult.Fail("Device not found.");

        device.DeviceType = deviceType.Trim();
        device.Brand = EmptyToNull(brand);
        device.SerialNumber = serialNumber.Trim();
        device.Password = EmptyToNull(password);
        try
        {
            _devices.Update(device);
            return SaveResult.Success(deviceId);
        }
        catch (Exception)
        {
            return SaveResult.Fail("Could not save the device. Please try again.");
        }
    }

    private static string? Validate(string deviceType, string serialNumber, string? brand, string? password)
    {
        if (string.IsNullOrWhiteSpace(deviceType))
            return "Please enter the device type (for example Laptop or Phone).";
        if (deviceType.Trim().Length > 45)
            return "Device type is too long (maximum 45 characters).";
        if (string.IsNullOrWhiteSpace(serialNumber))
            return "Please enter the device serial number.";
        if (serialNumber.Trim().Length > 45)
            return "Serial number is too long (maximum 45 characters).";
        if (!string.IsNullOrWhiteSpace(brand) && brand.Trim().Length > 45)
            return "Brand is too long (maximum 45 characters).";
        if (!string.IsNullOrWhiteSpace(password) && password.Trim().Length > 45)
            return "Password is too long (maximum 45 characters).";
        return null;
    }

    private static string? EmptyToNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        return value.Trim();
    }
}
