
namespace RepairTrack.Presentation;

public class PlaceholderScreen : UserControl
{
    public PlaceholderScreen(string title)
    {
        Dock = DockStyle.Fill;
        BackColor = Ui.WindowBackground;
        Controls.Add(new Label
        {
            Text = title + " (coming soon)",
            Font = Ui.TitleFont,
            ForeColor = Ui.TextPrimary,
            AutoSize = true,
            Location = new Point(24, 24)
        });
    }
}