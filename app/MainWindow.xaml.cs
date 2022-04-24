using System;
using System.Collections.Generic;
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

namespace app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            Resources.Add("TextIconColor", 
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF")));
            
            InitializeComponent();
        }
    }
}