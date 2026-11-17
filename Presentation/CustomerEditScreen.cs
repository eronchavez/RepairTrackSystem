using RepairTrack.Services;

namespace RepairTrack.Presentation;

public class CustomerEditScreen : UserControl
{
    private readonly int? _customerId;
    private readonly CustomerService _customers = new();
    private readonly DeviceService _devices = new();
    private readonly TextBox _name = Ui.Input();
    private readonly TextBox _phone = Ui.Input();
    private readonly TextBox _address = Ui.Input();
    private DataGridView? _deviceGrid;

    public CustomerEditScreen(int? customerId = null)
    {
        _customerId = customerId;
        _address.Multiline = true;
        _address.Height = 72;
        _address.ScrollBars = ScrollBars.Vertical;
        BuildUi();
        if (_customerId != null)
            LoadCustomer();
    }

    private void BuildUi()
    {
        Dock = DockStyle.Fill;
        BackColor = Ui.WindowBackground;
        Padding = new Padding(24);
        AutoScroll = true;

        string title = _customerId == null ? "Add Customer" : "Edit Customer";
        var save = Ui.PrimaryButton("Save", 100);
        var cancel = Ui.SecondaryButton("Cancel", 100, Ui.Back);
        save.Click += (s, e) => Save();
        cancel.Click += (s, e) =>
        {
            if (_customerId != null)
                Ui.Open(this, new CustomerDetailScreen(_customerId.Value));
            else
                Ui.Open(this, new CustomersScreen());
        };

        var header = Ui.Header(title, "Name is required. Phone and address are optional.", cancel, save);
        header.Dock = DockStyle.Top;

        var form = new Panel { Dock = DockStyle.Top, Height = 220, BackColor = Ui.PanelBackground, Padding = new Padding(16) };
        AddField(form, "Full name", _name, 0, true);
        AddField(form, "Phone", _phone, 56, false);
        Ui.AddCaption(form, "Address", false, new Point(16, 112));
        _address.Location = new Point(16, 132);
        form.Controls.Add(_address);

        if (_customerId != null)
            BuildDeviceSection();

        Controls.Add(form);
        Controls.Add(new Panel { Dock = DockStyle.Top, Height = 12 });
        Controls.Add(header);
    }

    private void BuildDeviceSection()
    {
        var addDevice = Ui.PrimaryButton("Add Device", 130, Ui.Add);
        addDevice.Click += (s, e) => Ui.Open(this, new DeviceEditScreen(_customerId!.Value, null, AfterDeviceSave.CustomerEdit));
        var editDevice = Ui.SecondaryButton("Edit Selected", 130, Ui.Edit);
        editDevice.Click += (s, e) => OpenSelectedDevice();

        var sectionHeader = Ui.Header("Devices", "Add or edit devices for this customer.", editDevice, addDevice);
        sectionHeader.Dock = DockStyle.Top;

        _deviceGrid = Ui.Grid();
        _deviceGrid.Columns.Add("type", "DEVICE TYPE");
        _deviceGrid.Columns.Add("brand", "BRAND");
        _deviceGrid.Columns.Add("serial", "SERIAL NUMBER");
        _deviceGrid.CellClick += (s, e) =>
        {
            if (e.RowIndex < 0 || _deviceGrid == null)
                return;
            if (_deviceGrid.Rows[e.RowIndex].Tag is int deviceId)
                Ui.Open(this, new DeviceDetailScreen(deviceId));
        };
        LoadDevices();

        var tablePanel = new Panel { Dock = DockStyle.Top, Height = 260, BackColor = Ui.PanelBackground, Padding = new Padding(8) };
        tablePanel.Controls.Add(_deviceGrid);

        Controls.Add(tablePanel);
        Controls.Add(new Panel { Dock = DockStyle.Top, Height = 8 });
        Controls.Add(sectionHeader);
    }

    private void LoadCustomer()
    {
        var customer = _customers.GetById(_customerId!.Value);
        if (customer == null)
        {
            MessageBox.Show("Customer not found.", "Repair Track", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Ui.Open(this, new CustomersScreen());
            return;
        }
        _name.Text = customer.FullName;
        _phone.Text = customer.Phone ?? "";
        _address.Text = customer.Address ?? "";
    }

    private void LoadDevices()
    {
        if (_deviceGrid == null || _customerId == null)
            return;
        _deviceGrid.Rows.Clear();
        foreach (var device in _devices.GetByCustomer(_customerId.Value))
        {
            int row = _deviceGrid.Rows.Add(device.DeviceType, device.Brand ?? "", device.SerialNumber);
            _deviceGrid.Rows[row].Tag = device.DeviceId;
        }
    }

    private void OpenSelectedDevice()
    {
        int? id = _deviceGrid == null ? null : Ui.SelectedId(_deviceGrid);
        if (id == null)
        {
            MessageBox.Show("Select a device first.", "Repair Track", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        Ui.Open(this, new DeviceEditScreen(_customerId!.Value, id, AfterDeviceSave.CustomerEdit));
    }

    private void Save()
    {
        SaveResult result = _customerId == null
            ? _customers.Add(_name.Text, _phone.Text, _address.Text)
            : _customers.Update(_customerId.Value, _name.Text, _phone.Text, _address.Text);

        if (!result.Ok)
        {
            MessageBox.Show(result.Message, "Please check the form", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (_customerId == null)
            Ui.Open(this, new CustomerEditScreen(result.Id));
        else
            Ui.Open(this, new CustomerDetailScreen(_customerId.Value));
    }

    private static void AddField(Panel form, string caption, TextBox box, int top, bool required)
    {
        Ui.AddCaption(form, caption, required, new Point(16, top));
        box.Location = new Point(16, top + 20);
        form.Controls.Add(box);
    }
}
