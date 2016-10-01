using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
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
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

using static YourAV.ManagementHelper;

namespace YourAV
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string AVName { get; set; } = "YourAV";
        public string AVGuid { get; set; } = Guid.NewGuid().ToString();
        public PackIconKind IconKind
            => IsAntivirusInstalled() ? PackIconKind.CheckboxMarkedCircleOutline : PackIconKind.AlertCircleOutline;
        public string ButtonContent
            => IsAntivirusInstalled() ? "关闭" : "开启";
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsAntivirusInstalled())
                RemoveAllAntivirus();
            else
                AddAntivirus(AVName, AVGuid);
            App.ChangeTheme();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IconKind)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ButtonContent)));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AVGuid = Guid.NewGuid().ToString();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AVGuid)));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
