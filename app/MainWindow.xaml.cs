using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Khawasu DataInstance;
        
        public MainWindow()
        {
            /* Palette for theme */
            Resources.Add("PrimaryTextColor",
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#212121")));
            Resources.Add("SecondaryTextColor",
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575")));
            Resources.Add("AccentColor", 
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5252")));
            Resources.Add("DividerColor",
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BDBDBD")));
            Resources.Add("DarkPrimaryColor",
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0097A7")));
            Resources.Add("LightPrimaryColor", 
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B2EBF2")));
            Resources.Add("PrimaryColor", 
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00BCD4")));
            Resources.Add("TextIconColor", 
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF")));

            
            InitializeComponent();
            //Style = (Style)FindResource(typeof(Window));
            Closing += CloseEvent;

            DataInstance = new Khawasu("localhost:8080");
            
            var deviceProcessing = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            deviceProcessing.Tick += redrawDevices;
            deviceProcessing.Start();
            
            AppMenu.ChangedOption += delegate(string param)
            {
                if (param == "Devices")
                {
                    DevicesTab.Visibility = Visibility.Visible;
                    LogTab.Visibility = Visibility.Collapsed;
                }
                else if(param == "Log")
                {
                    DevicesTab.Visibility = Visibility.Collapsed;
                    LogTab.Visibility = Visibility.Visible;
                }
            };
            
            var logProcessing = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            logProcessing.Tick += updateLog;
            logProcessing.Start();
        }

        void updateLog(object sender, EventArgs e)
        {
            string error;
            if (DataInstance.ErrorLog.TryDequeue(out error))
            {
                LogBox.Text += $"{DateTime.Now}: {error}\n";
            }
        }

        void redrawDevices(object? sender, EventArgs e)
        {
            // If get update devices signal
            if (DataInstance.UpdateDevicesSemaphore.WaitOne(1))
            {
                ListDevices.Items.Clear();
                ListSensors.Children.Clear();
            
                foreach (var device in DataInstance.Devices)
                {
                    if(device.Type == "Relay")
                        ListDevices.Items.Add(new Relay {DeviceData = device, DataInstance = DataInstance} );
                    else if (device.Type == "Temp-Sensor")
                    {
                        ListSensors.Children.Add(new TempHumSensor { DeviceData = device });
                    }
                    else
                        ListDevices.Items.Add(new device {DeviceData = device, DataInstance = DataInstance});
                
                    //Subscribe to update
                    DataInstance.Subscribes.Enqueue(device);
                }
            }
            
            // New subscribe data
            foreach (var sensor in ListSensors.Children)
            {
                var thsens = (TempHumSensor)sensor;
                object new_data;
                if (thsens.DeviceData.IncomingUpdates.TryDequeue(out new_data))
                {
                    thsens.UpdateStatus(new_data);
                }
            }
            
            foreach (var dev in ListDevices.Items)
            {
                var device = (device)dev;
                object new_data;
                if (DataInstance.GetDeviceByAddress(device.Address)!.Value!.IncomingUpdates.TryDequeue(out new_data))
                {
                    device.UpdateStatus(new_data);
                }
            }
        }

        void CloseEvent(object sender, CancelEventArgs e)
        {
            // Stop all threads
            DataInstance.ThreadState.Release(1);
        }
    }
}