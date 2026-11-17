using RepairTrack.Data.Entities;
using RepairTrack.Services;

namespace RepairTrack.Presentation;

public class JobOrderNewScreen : UserControl
{
    private readonly int? _deviceId;
    private readonly DeviceService _devices = new();
    private readonly JobOrderService _jobs = new();
    private readonly ComboBox _deviceList = new()
    {
        DropDownStyle = ComboBoxStyle.DropDownList,
        Width = 480,
        Height = 28,
        Font = Ui.BodyFont
    };
    private readonly TextBox _issue = Ui.Input(480);
    private readonly TextBox _cost = Ui.Input(180);

    public JobOrderNewScreen(int? deviceId = null)
    {
        _deviceId = deviceId;
        _issue.Multiline = true;
        _issue.Height = 90;
        _issue.ScrollBars = ScrollBars.Vertical;
        BuildUi();
        LoadDevices();
    }

    private void BuildUi()
    {
        Dock = DockStyle.Fill;
        BackColor = Ui.WindowBackground;
        Padding = new Padding(24);

        var save = Ui.PrimaryButton("Create job", 130);
        var cancel = Ui.SecondaryButton("Cancel", 100, Ui.Back);
        save.Click += (s, e) => Save();
        cancel.Click += (s, e) =>
        {
            if (_deviceId != null)
                Ui.Open(this, new DeviceDetailScreen(_deviceId.Value));
            else
                Ui.Open(this, new JobOrdersScreen());
        };

        var header = Ui.Header(
            "New Job Order",
            "New jobs start as Received. Issue description is required. Total cost can wait.",
            cancel,
            save);

        var form = new Panel { Dock = DockStyle.Top, Height = 320, BackColor = Ui.PanelBackground, Padding = new Padding(16) };
        Ui.AddCaption(form, "Device", true, new Point(16, 0));
        _deviceList.Location = new Point(16, 20);
        _deviceList.BackColor = Ui.PanelBackground;
        _deviceList.ForeColor = Ui.TextDefault;
        form.Controls.Add(_deviceList);

        Ui.AddCaption(form, "Issue description", true, new Point(16, 56));
        _issue.Location = new Point(16, 76);
        form.Controls.Add(_issue);

        Ui.AddCaption(form, "Total cost (optional)", false, new Point(16, 176));
        _cost.PlaceholderText = "0.00";
        _cost.Location = new Point(16, 196);
        form.Controls.Add(_cost);

        Controls.Add(form);
        Controls.Add(new Panel { Dock = DockStyle.Top, Height = 12 });
        Controls.Add(header);
    }

    private void LoadDevices()
    {
        _deviceList.Items.Clear();
        foreach (var device in _devices.GetAll())
        {
            var item = new DeviceChoice(device);
            _deviceList.Items.Add(item);
            if (_deviceId != null && device.DeviceId == _deviceId)
                _deviceList.SelectedItem = item;
        }
        if (_deviceList.SelectedItem == null && _deviceList.Items.Count > 0)
            _deviceList.SelectedIndex = 0;
    }

    private void Save()
    {
        if (_deviceList.SelectedItem is not DeviceChoice choice)
        {
            MessageBox.Show("Please select a device.", "Please check the form", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        decimal cost = 0;
        if (!string.IsNullOrWhiteSpace(_cost.Text))
        {
            if (!Ui.TryParseMoney(_cost.Text, out cost) || cost < 0)
            {
                MessageBox.Show("Enter a valid total cost, or leave it blank.", "Please check the form", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        var result = _jobs.Create(choice.DeviceId, _issue.Text, cost);
        if (!result.Ok)
        {
            MessageBox.Show(result.Message, "Please check the form", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        Ui.Open(this, new JobOrderDetailScreen(result.Id));
    }

    private class DeviceChoice
    {
        public int DeviceId { get; }
        private readonly string _text;

        public DeviceChoice(Device device)
        {
            DeviceId = device.DeviceId;
            _text = device.Customer.FullName + "  —  " + Ui.DeviceTitle(device.DeviceType, device.Brand) + "  (" + device.SerialNumber + ")";
        }

        public override string ToString()
        {
            return _text;
        }
    }
}
