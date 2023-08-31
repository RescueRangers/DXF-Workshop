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
using CommunityToolkit.Mvvm.ComponentModel;
using GongSolutions.Wpf.DragDrop;
using CommunityToolkit.Mvvm.Input;

namespace DXF_Light.ViewModel
{
    public class NoContourDxfViewModel : ObservableObject, IDropTarget
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
        private int _height;

        public double CutLength
        {
            get => _cutLength;
            set => SetProperty(ref _cutLength, value);
        }

        public int NumberOfCuts
        {
            get => _numberOfCuts;
            set => SetProperty(ref _numberOfCuts, value);
        }

        public NoContourDxf NoContourDxf
        {
            get => _noContourDxf;
            set => SetProperty(ref _noContourDxf, value);
        }

        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        public int Height
        {
            get => _height; set
            {
				SetProperty(ref _height, value);
            }
        }

        public string Delimiter
        {
            get => _delimiter;
            set => SetProperty(ref _delimiter, value);
        }

        public bool Headers
        {
            get => _headers;
            set => SetProperty(ref _headers, value);
        }

        public NoContourDxfViewModel(IDataService dataService, IIOService ioService)
        {
            _dataService = dataService;
            _ioService = ioService;
            LoadCommands();
        }

        private void LoadCommands()
        {
            CreateNcDxfCommand = new RelayCommand(CreateNcDxf, () => NoContourDxf.IsValid && Height > 0);
            AddCutsCommand = new RelayCommand(AddCuts, () => CutLength > 0 && NumberOfCuts > 0);
            LoadCutsCommand = new RelayCommand(LoadCuts, () => true);
        }

        private void LoadCuts()
        {
            var defaultPath = string.IsNullOrWhiteSpace(Settings.Default.InitialFolder)
                ? _appFilePath
                : Settings.Default.InitialFolder;

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
                    _ioService.Message(exception.Message, Resources.Error);
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
                        _ioService.Message(error.Message, Resources.Error);
                        _savePath = null;
                        return;
                    }

                    _savePath = item;
                });

            if (_savePath == null) return;
            NoContourDxf.Height = Height;
            await Task.Run(() => _ioService.CreateNCDxf((error) =>
            {
                if (error != null)
                {
                    _ioService.Message(error.Message, Resources.Error);
                }

                _ioService.Message(Resources.Success + _savePath, Resources.FileOperation);
            }, NoContourDxf, _savePath)).ConfigureAwait(false);

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
                var fInfo = new FileInfo(dragFileList.First());
                NoContourDxf.Name = fInfo.Name.Remove(fInfo.Name.LastIndexOf('.'));
                ReadCsv();
            }
        }

		public void DragEnter(IDropInfo dropInfo)
		{
		}

		public void DragLeave(IDropInfo dropInfo)
		{
		}
	}
}