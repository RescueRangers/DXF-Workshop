using System;
using System.Collections.Generic;
using System.Windows;
using DXF_Light.ViewModel;

namespace DXF_Light
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        //static App()
        //{
        //    DispatcherHelper.Initialize();
        //}

        private const string Unique = "DXF_Workshop_By_Radoslaw_Radomski";

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();

                SingleInstance<App>.Cleanup();
            }
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            if (args.Count <= 0) return true;
            var viewModel = (MainViewModel)Current.MainWindow.DataContext;
            viewModel.StartupArgument = args[1];

            return true;
        }
    }
}