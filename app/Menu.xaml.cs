using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace app;

public partial class Menu : UserControl
{
    public string Value = "Devices";
    public delegate void ChangeOption(string sampleParam);
    public event ChangeOption ChangedOption;
    
    public Menu()
    {
        InitializeComponent();
        DataContext = this;
        
        List.Items.Clear();
        List.Items.Add(CreateButton("Devices", selectMenu));
        List.Items.Add(CreateButton("Log", selectMenu));
    }

    private void selectMenu(object sender, EventArgs args)
    {
        Value = (string)((Button)sender).Content;
        foreach (Button button in List.Items)
        {
            SetStyleToButton(button, (string)button.Content == Value);
        }

        ChangedOption(Value);
    }

    private void SetStyleToButton(Button button, bool state)
    {
        if (state)
        {
            button.Background =
                (System.Windows.Media.SolidColorBrush)Application.Current.MainWindow.Resources["DarkPrimaryColor"];
            button.Foreground =
                (System.Windows.Media.SolidColorBrush)Application.Current.MainWindow.Resources["TextIconColor"];
        }
        else
        {
            button.Background =
                (System.Windows.Media.SolidColorBrush)Application.Current.MainWindow.Resources["PrimaryColor"];
            button.Foreground =
                (System.Windows.Media.SolidColorBrush)Application.Current.MainWindow.Resources["TextIconColor"];
        }
    }
    
    // Create styles button with specific text and click handler
    protected Button CreateButton(string text, RoutedEventHandler action)
    {
        var btn = new Button
        {
            Content = text,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(10),
            FontSize = 20,
            FontWeight = FontWeights.Light
            
        };

        SetStyleToButton(btn, text == Value);

        btn.Click += action;

        return btn;
    }
}