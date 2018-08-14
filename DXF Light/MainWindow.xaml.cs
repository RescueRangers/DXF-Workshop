using System;
using System.Windows;
using DXF_Light.ViewModel;
using MahApps.Metro;
using MahApps.Metro.Controls;

namespace DXF_Light
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Accent _accentLight = ThemeManager.DetectAppStyle(Application.Current).Item2;
        private readonly Accent _accentDark = ThemeManager.GetAccent("Olive");

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private void ToggleSwitch_OnIsCheckedChanged(object sender, EventArgs e)
        {
            var toggleSwitch = (ToggleSwitch) sender;
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            if (toggleSwitch.IsChecked == true)
            {
                ThemeManager.ChangeAppStyle(Application.Current, _accentDark, ThemeManager.GetAppTheme("BaseDark"));
            }
            else
            {
                ThemeManager.ChangeAppStyle(Application.Current, _accentLight, ThemeManager.GetAppTheme("BaseLight"));
            }
        }
    }
}