using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

using static YourAV.ThemeHelper;
using static YourAV.ManagementHelper;

namespace YourAV
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static List<Swatch> SwatchList { get; } = new SwatchesProvider().Swatches.ToList();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ChangeTheme();
        }
        public static void ChangeTheme()
        {
            ApplyPrimary(IsAntivirusInstalled() ? SwatchList.Where(item => item.Name == "teal").FirstOrDefault() : SwatchList.Where(item => item.Name == "red").FirstOrDefault());
        }
    }
}
