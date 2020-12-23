using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using DXF_Light.Model;
using DXF_Light.Servicess;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using WPFLocalizeExtension.Engine;

namespace DXF_Light.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IIOService _ioService;
        private List<string> _arguments;

        public string StartupArgument
        {
            set
            {
                _startupArgument = value;
                AddDxfsFromPath(_startupArgument);
            }
        }
        
        public bool Pinned
        {
            get { return _pinned; }
            set
            {
                if (_pinned != value)
                {
                    _pinned = value;
                    RaisePropertyChanged(nameof(Pinned));
                    RaisePropertyChanged(nameof(PinIcon));
                }
            }
        }

        public string Version { get; set; }

        public string PinIcon { get => Pinned ? "Pin" : "PinOff"; }

        private void AddDxfsFromPath(string startupArgument)
        {
            var arguments = new List<string> { startupArgument };
            _ioService.GetDxfFilesFromPaths((dxfFiles, directory, exception) =>
            {
                if (exception != null)
                {
                    _ioService.Message(exception.Message, Properties.Resources.Error);
                    return;
                }

                var tab = Children[0] as PlxTabViewModel;

                tab?.PlxFile.DxfFiles.AddRange(dxfFiles);
                tab.SavePath = directory;
            }, arguments);
        }

        private static string _startupArgument;
        private ObservableCollection<object> _children;
        private bool _pinned;

        public ObservableCollection<object> Children { get { return _children; } }

        public ICommand ExitCommand { get; private set; }
        public ICommand EnglishCommand { get; private set; }
        public ICommand PolishCommand { get; private set; }
        public ICommand LoadedCommand { get; private set; }
        public ICommand TogglePinCommand { get; private set; }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _ioService = new IOService();
            LoadCommands();

            _children = new ObservableCollection<object>
            {
                new PlxTabViewModel(_ioService),
                new DxfTabViewModel(dataService, _ioService),
                new PlyTabVewModel(dataService, _ioService),
                new NoContourDxfViewModel(dataService, _ioService),
                new XinTabViewModel(dataService, _ioService)
            };

            Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        private void LoadCommands()
        {
            ExitCommand = new RelayCommand(Exit, () => true);
            PolishCommand = new RelayCommand(Polish, () => true);
            EnglishCommand = new RelayCommand(English, () => true);
            LoadedCommand = new RelayCommand(Loaded, () => true);
            TogglePinCommand = new RelayCommand(TogglePin, () => true);
        }

        private void TogglePin()
        {
            Pinned = !Pinned;
        }

        private void Loaded()
        {
            _arguments = Environment.GetCommandLineArgs().ToList();

            if (_arguments.Count > 1)
            {
                _arguments = _arguments.Skip(1).ToList();

                _ioService.GetDxfFilesFromPaths((dxfFiles, directory, exception) =>
                {
                    if (exception != null)
                    {
                        _ioService.Message(exception.Message, Properties.Resources.Error);
                        return;
                    }

                    var tab = Children[0] as PlxTabViewModel;

                    tab.PlxFile = new PlxFile(dxfFiles);
                    tab.SavePath = directory;
                }, _arguments);
            }
        }

        private void English()
        {
            LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
            LocalizeDictionary.Instance.Culture = new CultureInfo("en");
        }

        private void Polish()
        {
            LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
            LocalizeDictionary.Instance.Culture = new CultureInfo("pl");
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}