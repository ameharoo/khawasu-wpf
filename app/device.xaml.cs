using System.Windows;
using System.Windows.Controls;

namespace app;

public partial class device : UserControl
{
    public string Title { get; set; } = "Unknown";
    public string Type { get; set; } = "Unknown";
    public string Address { get; set; } = "00:00:00:00:00/00";

    public Khawasu DataInstance;    

    private Device _deviceData;
    
    public Device DeviceData
    {
        get => _deviceData;
        set
        {
            updateData(value);
            _deviceData = value;
        }
    }

    public device()
    {
        InitializeComponent();

        DataContext = this;
        SetActions();
    }

    // Create styles button with specific text and click handler
    protected Button CreateButton(string text, RoutedEventHandler action, bool state=true)
    {
        var btn = new Button
        {
            Width = 106,
            Height = 50,
            Margin = new Thickness(10),
            Background = state ? (System.Windows.Media.SolidColorBrush) Application.Current.MainWindow.Resources["AccentColor"] :
                (System.Windows.Media.SolidColorBrush) Application.Current.MainWindow.Resources["DarkPrimaryColor"],
            Foreground = (System.Windows.Media.SolidColorBrush) Application.Current.MainWindow.Resources["TextIconColor"],
            Content = text,
            BorderThickness = new Thickness(0)
        };

        btn.Click += action;

        return btn;
    }

    protected virtual void SetActions()
    {
        ActionStack.Children.Clear();
    }
    protected void updateData(Device data)
    {
        Title = data.Name;
        Type = data.Type;
        Address = data.Address;
    }
    public virtual void UpdateStatus(object data)
    {
        
    }
}