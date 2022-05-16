using System;
using System.Diagnostics;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace app;

public class Relay : device
{
    public bool _state = false;
    public bool State
    {
        get => _state;
        set
        {
            _state = value;
            SetActions();
        }
    }
    
    public Relay()
    {
        Type = "Relay";
        InitializeComponent();
        SetActions();
    }

    private void HandleState(object sender, RoutedEventArgs e)
    {
        // Send new state
        DataInstance.GetDeviceByAddress(Address).Value.OutcomingUpdates.Enqueue(State ? "0" : "1");
    }

    public override void UpdateStatus(object status)
    {
        try
        {
            State = (int)((JObject)status)["status"] == 1;
        }
        catch (Exception e)
        {
            
        }
    }
    
    protected override void SetActions()
    {
        Trace.WriteLine(State);
        ActionStack.Children.Clear();
        ActionStack.Children.Add(CreateButton(State? "Off" : "On", HandleState, State));
    }
}