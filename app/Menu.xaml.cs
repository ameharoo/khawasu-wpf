using System.Windows;
using System.Windows.Controls;

namespace app;

public partial class Menu : UserControl
{
    public Menu()
    {
        InitializeComponent();
        DataContext = this;
        
        List.Items.Clear();
        List.Items.Add(CreateButton("Devices", (sender, args) => { }));
    }
    
    // Create styles button with specific text and click handler
    protected Button CreateButton(string text, RoutedEventHandler action)
    {
        var btn = new Button
        {
            Background = (System.Windows.Media.SolidColorBrush) Application.Current.MainWindow.Resources["DarkPrimaryColor"],
            Foreground = (System.Windows.Media.SolidColorBrush) Application.Current.MainWindow.Resources["TextIconColor"],
            Content = text,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(10),
            FontSize = 20,
            FontWeight = FontWeights.Light
            
        };

        btn.Click += action;

        return btn;
    }
}