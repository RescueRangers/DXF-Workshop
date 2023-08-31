using System;
using System.Collections.Generic;
using System.Windows;
using DXF_Light.Model;
using DXF_Light.Servicess;
using DXF_Light.ViewModel;
using Microsoft.Extensions.DependencyInjection;

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
		public IServiceProvider Services { get; set; }
		public new static App Current => (App)Application.Current;

		[STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();
				application.Services = ConfigureServices();
				application.InitializeComponent();
                application.Run();

                SingleInstance<App>.Cleanup();
            }
        }

		private static IServiceProvider ConfigureServices()
		{
			var services = new ServiceCollection();

			services.AddSingleton<IDataService, DataService>();
			services.AddSingleton<IIOService, IOService>();

            services.AddTransient<MainViewModel>();

			return services.BuildServiceProvider();
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