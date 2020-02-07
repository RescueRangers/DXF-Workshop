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
    public class DxfTabViewModel : ViewModelBase, IDropTarget
    {
        private string _filePath;
        private string _delimiter = ";";
        private bool _headers;
        private ObservableCollection<DxfFile> _dxfFiles = new ObservableCollection<DxfFile>();

        private readonly string _appFilePath = AppDomain.CurrentDomain.BaseDirectory;
        private const string CsvFiles = "Pliki csv (.csv)|*.csv";

        private readonly IDataService _dataService;
        private readonly IIOService _ioService;
        private string _savePath;

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

        public ObservableCollection<DxfFile> DxfFiles
        {
            get => _dxfFiles;
            set => Set(ref _dxfFiles, value);
        }

        public ICommand GetFilePathCommand { get; private set; }
        public ICommand CreateDxfsCommand { get; private set; }

        public DxfTabViewModel(IDataService dataService, IIOService ioService)
        {
            _dataService = dataService;
            _ioService = ioService;
            LoadCommands();
        }

        private void LoadCommands()
        {
            GetFilePathCommand = new RelayCommand(GetFilePath, () => true);
            CreateDxfsCommand = new RelayCommand(CreateDxfs, () => DxfFiles != null && DxfFiles.Count > 0);
        }

        private async void CreateDxfs()
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

            await Task.Run(() => _ioService.CreateDxfFiles((error) =>
            {
                if (error != null)
                {
                    _ioService.Message(error.Message, Properties.Resources.Error);
                }

                _ioService.Message(Properties.Resources.Success + _savePath, Properties.Resources.FileOperation);
            }, DxfFiles.ToList(), _savePath));

            DxfFiles.Clear();
        }

        private void GetFilePath()
        {
            var defaultPath = string.IsNullOrWhiteSpace(Properties.Settings.Default.InitialFolder)
                ? _appFilePath
                : Properties.Settings.Default.InitialFolder;

            FilePath = _ioService.OpenFileDialog(defaultPath, CsvFiles);

            if (!string.IsNullOrWhiteSpace(FilePath)) ReadCsv();
        }

        private void ReadCsv()
        {
            _dataService.GetData(
                (items, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        _ioService.Message(error.Message, Properties.Resources.Error);
                        return;
                    }

                    DxfFiles = new ObservableCollection<DxfFile>(items);
                }, _filePath.Trim('"'), _delimiter, _headers ? 1 : 0);

            Settings.Default.InitialFolder = new FileInfo(_filePath.Trim('"')).DirectoryName;
            Settings.Default.Save();
        }

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