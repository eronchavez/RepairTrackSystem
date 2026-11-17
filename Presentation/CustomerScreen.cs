using RepairTrack.Services;

namespace RepairTrack.Presentation;

public class CustomersScreen : UserControl
{
    private readonly CustomerService _customers = new();
    private readonly DataGridView _grid = Ui.Grid();

    public CustomersScreen()
    {
        BuildUi();
        LoadRows();
    }

    private void BuildUi()
    {
        Dock = DockStyle.Fill;
        BackColor = Ui.WindowBackground;
        Padding = new Padding(24);

        var add = Ui.PrimaryButton(" Add Customer", 160, Ui.Add);
        var edit = Ui.SecondaryButton("Edit Selected", 140, Ui.Edit);
        add.Click += (s, e) => Ui.Open(this, new CustomerEditScreen());
        edit.Click += (s, e) => OpenEdit();

        var header = Ui.Header(
            "Customers",
            "Manage customer records and registered devices.",
            edit,
            add);

        var spacer = new Panel { Dock = DockStyle.Top, Height = 8 };

        _grid.Columns.Add("id", "CUSTOMER ID");
        _grid.Columns.Add("name", "FULL NAME");
        _grid.Columns.Add("phone", "PHONE");
        _grid.Columns.Add("address", "ADDRESS");
        _grid.Columns.Add("devices", "DEVICES");
        _grid.CellClick += (s, e) => OpenDetail(e.RowIndex);

        var tablePanel = new Panel { Dock = DockStyle.Fill, BackColor = Ui.PanelBackground, Padding = new Padding(8) };
        tablePanel.Controls.Add(_grid);

        Controls.Add(tablePanel);
        Controls.Add(spacer);
        Controls.Add(header);
    }

    private void LoadRows()
    {
        _grid.Rows.Clear();
        foreach (var customer in _customers.GetAll())
        {
            int row = _grid.Rows.Add(
                IdFormatter.CustomerId(customer.CustomerId),
                customer.FullName,
                customer.Phone ?? "",
                customer.Address ?? "",
                _customers.DeviceCount(customer).ToString());
            _grid.Rows[row].Tag = customer.CustomerId;
        }
    }

    private void OpenDetail(int rowIndex)
    {
        if (rowIndex < 0)
            return;
        if (_grid.Rows[rowIndex].Tag is int id)
            Ui.Open(this, new CustomerDetailScreen(id));
    }

    private void OpenEdit()
    {
        int? id = Ui.SelectedId(_grid);
        if (id == null)
        {
            MessageBox.Show("Select a customer first.", "Repair Track", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        Ui.Open(this, new CustomerEditScreen(id));
    }
}
