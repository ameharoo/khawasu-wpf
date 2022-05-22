using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace app;

public partial class TempHumSensor : UserControl
{
    private double _temp = 20;
    private double _hum = 20;
    private string _name = "";
    private double bestTemp = 24;
    private double bestHum = 65;

    public Device DeviceData;
    public Khawasu InstKhawasu;

    public string NameValue
    {
        get => _name;
        set
        {
            _name = value;
            NameLabel.Content = value;
        }
    }

    public double TemperatureValue
    {
        get => _temp;
        set { 
            _temp = value;
            if (value > bestTemp - 5 && value < bestTemp + 5)
            {
                TempBar.Value = value / bestTemp * 100;
                TempBar.Foreground = (System.Windows.Media.SolidColorBrush) Application.Current.MainWindow.Resources["PrimaryColor"];
                TempBar.OuterBackgroundBrush = (System.Windows.Media.SolidColorBrush) Application.Current.MainWindow.Resources["LightPrimaryColor"];
            }else if (value < bestTemp - 5)
            {
                TempBar.Value = Math.Max(value, 0);
                TempBar.Foreground = (System.Windows.Media.SolidColorBrush) Application.Current.MainWindow.Resources["AccentColor"];
                TempBar.OuterBackgroundBrush = (System.Windows.Media.SolidColorBrush) Application.Current.MainWindow.Resources["DividerColor"];
            }
            else
            {
                TempBar.Value = 100;
                TempBar.Foreground = (System.Windows.Media.SolidColorBrush) Application.Current.MainWindow.Resources["AccentColor"];
                TempBar.OuterBackgroundBrush = (System.Windows.Media.SolidColorBrush) Application.Current.MainWindow.Resources["DividerColor"];
            }

            
            TemperatureLabel.Content = String.Format("{0:00.0}", value);
        }
    }
    
    public double HumidityValue
    {
        get => _hum;
        set { 
            _hum = value;
            if (value > bestHum - 20 && value < bestHum + 20)
            {
                HumBar.Value = value / bestHum * 100;
                HumBar.Foreground = (System.Windows.Media.SolidColorBrush) Application.Current.MainWindow.Resources["PrimaryColor"];
                HumBar.OuterBackgroundBrush = (System.Windows.Media.SolidColorBrush) Application.Current.MainWindow.Resources["LightPrimaryColor"];
            }else if (value < bestHum - 20)
            {
                HumBar.Value = Math.Max(value, 0);
                HumBar.Foreground = (System.Windows.Media.SolidColorBrush) Application.Current.MainWindow.Resources["AccentColor"];
                HumBar.OuterBackgroundBrush = (System.Windows.Media.SolidColorBrush) Application.Current.MainWindow.Resources["DividerColor"];
            }
            else
            {
                HumBar.Value = 100;
                HumBar.Foreground = (System.Windows.Media.SolidColorBrush) Application.Current.MainWindow.Resources["AccentColor"];
                HumBar.OuterBackgroundBrush = (System.Windows.Media.SolidColorBrush) Application.Current.MainWindow.Resources["DividerColor"];
            }
            
            
            HumidityLabel.Content = String.Format("{0:00.00}",value);
        }
    }
    public TempHumSensor()
    {
        InitializeComponent();

        TemperatureValue = 0;
        HumidityValue = 0;
        NameValue = DeviceData.Name;

        //InstKhawasu = instKhawasu;
        

    }
    
    public void UpdateStatus(object data)
    {
        try
        {
            HumidityValue = (double)((JObject)data)["humidity"];
            TemperatureValue = (double)((JObject)data)["temperature"];
        }
        catch (Exception e)
        {
            
        }
    }
}