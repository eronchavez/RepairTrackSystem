using RepairTrack.Services;

namespace RepairTrack.Presentation;

public enum AfterDeviceSave
{
    CustomerEdit,
    CustomerDetail,
    DeviceDetail
}

public class DeviceEditScreen : UserControl
{
    private readonly int _customerId;
    private readonly int? _deviceId;
    private readonly AfterDeviceSave _afterSave;
    private readonly DeviceService _devices = new();
    private readonly CustomerService _customers = new();
    private readonly TextBox _type = Ui.Input();
    private readonly TextBox _brand = Ui.Input();
    private readonly TextBox _serial = Ui.Input();
    private readonly TextBox _password = Ui.Input();

    public DeviceEditScreen(int customerId, int? deviceId, AfterDeviceSave afterSave)
    {
        _customerId = customerId;
        _deviceId = deviceId;
        _afterSave = afterSave;
        BuildUi();
        if (_deviceId != null)
            LoadDevice();
    }

    private void BuildUi()
    {
        Dock = DockStyle.Fill;
        BackColor = Ui.WindowBackground;
        Padding = new Padding(24);

        var customer = _customers.GetById(_customerId);
        string owner = customer?.FullName ?? "customer";
        string title = _deviceId == null ? "Add Device" : "Edit Device";

        var save = Ui.PrimaryButton("Save", 100);
        var cancel = Ui.SecondaryButton("Cancel", 100, Ui.Back);
        save.Click += (s, e) => Save();
        cancel.Click += (s, e) => GoBack(_deviceId);

        var header = Ui.Header(title, "For " + owner + ". Serial number is required.", cancel, save);

        var form = new Panel { Dock = DockStyle.Top, Height = 280, BackColor = Ui.PanelBackground, Padding = new Padding(16) };
        AddField(form, "Device type", _type, 0, true);
        AddField(form, "Brand", _brand, 56, false);
        AddField(form, "Serial number", _serial, 112, true);
        AddField(form, "Password", _password, 168, false);

        Controls.Add(form);
        Controls.Add(new Panel { Dock = DockStyle.Top, Height = 12 });
        Controls.Add(header);
    }

    private void LoadDevice()
    {
        var device = _devices.GetById(_deviceId!.Value);
        if (device == null)
        {
            MessageBox.Show("Device not found.", "Repair Track", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GoBack(null);
            return;
        }
        _type.Text = device.DeviceType;
        _brand.Text = device.Brand ?? "";
        _serial.Text = device.SerialNumber;
        _password.Text = device.Password ?? "";
    }

    private void Save()
    {
        SaveResult result = _deviceId == null
            ? _devices.Add(_customerId, _type.Text, _brand.Text, _serial.Text, _password.Text)
            : _devices.Update(_deviceId.Value, _type.Text, _brand.Text, _serial.Text, _password.Text);

        if (!result.Ok)
        {
            MessageBox.Show(result.Message, "Please check the form", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int deviceId = _deviceId ?? result.Id;
        GoBack(deviceId);
    }

    private void GoBack(int? deviceId)
    {
        switch (_afterSave)
        {
            case AfterDeviceSave.CustomerEdit:
                Ui.Open(this, new CustomerEditScreen(_customerId));
                break;
            case AfterDeviceSave.DeviceDetail when deviceId != null:
                Ui.Open(this, new DeviceDetailScreen(deviceId.Value));
                break;
            default:
                Ui.Open(this, new CustomerDetailScreen(_customerId));
                break;
        }
    }

    private static void AddField(Panel form, string caption, TextBox box, int top, bool required)
    {
        Ui.AddCaption(form, caption, required, new Point(16, top));
        box.Location = new Point(16, top + 20);
        form.Controls.Add(box);
    }
}
