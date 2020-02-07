using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DXF_Light.Model;
using DXF_Light.Properties;
using DXF_Light.Servicess;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GongSolutions.Wpf.DragDrop;

namespace DXF_Light.ViewModel
{
    public class NoContourDxfViewModel : ViewModelBase, IDropTarget
    {
        private readonly IDataService _dataService;
        private readonly IIOService _ioService;
        private readonly string _appFilePath = AppDomain.CurrentDomain.BaseDirectory;
        private bool _headers;
        private string _delimiter = ";";
        private string _filePath;
        private NoContourDxf _noContourDxf = new NoContourDxf();
        private int _numberOfCuts;
        private double _cutLength;
        private const string CsvFiles = "Pliki csv (.csv)|*.csv";
        private string _savePath;

        public double CutLength
        {
            get => _cutLength;
            set => Set(ref _cutLength, value);
        }

        public int NumberOfCuts
        {
            get => _numberOfCuts;
            set => Set(ref _numberOfCuts, value);
        }

        public NoContourDxf NoContourDxf
        {
            get => _noContourDxf;
            set => Set(ref _noContourDxf, value);
        }

        public string FilePath
        {
            get => _filePath;
            set => Set(ref _filePath, value);
        }

        public string Delimiter
        {
            get => _delimiter;
            set => Set(ref _delimiter, value);
        }

        public bool Headers
        {
            get => _headers;
            set => Set(ref _headers, value);
        }

        public NoContourDxfViewModel(IDataService dataService, IIOService ioService)
        {
            _dataService = dataService;
            _ioService = ioService;
            LoadCommands();
        }

        private void LoadCommands()
        {
            CreateNcDxfCommand = new RelayCommand(CreateNcDxf, () => NoContourDxf.IsValid);
            AddCutsCommand = new RelayCommand(AddCuts, () => CutLength > 0 && NumberOfCuts > 0);
            LoadCutsCommand = new RelayCommand(LoadCuts, () => true);
        }

        private void LoadCuts()
        {
            var defaultPath = string.IsNullOrWhiteSpace(Properties.Settings.Default.InitialFolder)
                ? _appFilePath
                : Properties.Settings.Default.InitialFolder;

            FilePath = _ioService.OpenFileDialog(defaultPath, CsvFiles);

            if (string.IsNullOrWhiteSpace(FilePath))
                return;
            ReadCsv();
        }

        private void ReadCsv()
        {
            _dataService.GetNCDxfData(((cuts, exception) =>
            {
                if (exception != null)
                {
                    _ioService.Message(exception.Message, Properties.Resources.Error);
                    return;
                }

                NoContourDxf.InternalCuts = new ObservableCollection<InternalCut>(cuts);
            }), _filePath, _delimiter, _headers ? 1 : 0);

            Settings.Default.InitialFolder = new FileInfo(_filePath).DirectoryName;
            Settings.Default.Save();
        }

        private void AddCuts()
        {
            var cuts = Enumerable.Repeat(CutLength, NumberOfCuts);
            NoContourDxf.AddCuts(cuts);
        }

        private async void CreateNcDxf()
        {
            _ioService.GetFolder(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        _ioService.Message(error.Message, Properties.Resources.Error);
                        _savePath = null;
                        return;
                    }

                    _savePath = item;
                });

            if (_savePath == null) return;

            await Task.Run(() => _ioService.CreateNCDxf((error) =>
            {
                if (error != null)
                {
                    _ioService.Message(error.Message, Properties.Resources.Error);
                }

                _ioService.Message(Properties.Resources.Success + _savePath, Properties.Resources.FileOperation);
            }, NoContourDxf, _savePath));

            NoContourDxf = new NoContourDxf();
        }

        public ICommand CreateNcDxfCommand { get; set; }
        public ICommand AddCutsCommand { get; set; }
        public ICommand LoadCutsCommand { get; set; }

        public void DragOver(IDropInfo dropInfo)
        {
            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = dragFileList.Any(item =>
            {
                var extension = Path.GetExtension(item);
                return extension != null && extension.Equals(".csv");
            }) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        public void Drop(IDropInfo dropInfo)
        {
            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>().ToList();

            if (dragFileList.Any(item =>
            {
                var extension = Path.GetExtension(item);
                return extension != null && extension.Equals(".csv");
            }))
            {
                _filePath = dragFileList.First();
                ReadCsv();
            }
        }
    }
}