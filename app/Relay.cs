using System.Windows;

namespace app;

public class Relay : device
{

    public Relay()
    {
        Type = "Relay";
        InitializeComponent();
        SetActions();
    }

    private void HandleState(object sender, RoutedEventArgs e)
    {
        
    }
    
    protected override void SetActions()
    {
        ActionStack.Children.Clear();
        ActionStack.Children.Add(CreateButton("On", HandleState));
    }
}