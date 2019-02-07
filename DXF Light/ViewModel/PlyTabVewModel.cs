using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DXF_Light.Model;
using DXF_Light.Servicess;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GongSolutions.Wpf.DragDrop;

namespace DXF_Light.ViewModel
{
    public class PlyTabVewModel : ViewModelBase, IDropTarget
    {
        private readonly string _appFilePath = AppDomain.CurrentDomain.BaseDirectory;
        private const string CsvFiles = "Pliki csv (.csv)|*.csv";

        private readonly IDataService _dataService;
        private readonly IIOService _ioService;

        private ObservableCollection<PlyFile> _plyFiles = new ObservableCollection<PlyFile>();
        private string _plyFilePath;
        private string _delimiter = ";";
        private bool _headers;
        private string _savePath;

        public string PlyFilePath
        {
            get => _plyFilePath;
            set => Set(nameof(PlyFilePath), ref _plyFilePath, value);
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

        public ObservableCollection<PlyFile> PlyFiles
        {
            get => _plyFiles;
            set => Set(nameof(PlyFiles), ref _plyFiles, value);
        }

        public PlyTabVewModel(IDataService dataService, IIOService ioService)
        {
            _dataService = dataService;
            _ioService = ioService;
            LoadCommands();
        }

        public ICommand CreatePliesCommand { get; set; }
        public ICommand GetPlyFilePathCommand { get; set; }
        public ICommand ReadPlyFileCommand { get; set; }

        private void LoadCommands()
        {
            CreatePliesCommand = new RelayCommand(CreatePlies, () => PlyFiles != null && PlyFiles.Any());
            GetPlyFilePathCommand = new RelayCommand(GetPlyFilePath, () => true);
            ReadPlyFileCommand = new RelayCommand(ReadPlyCsv, () => !string.IsNullOrWhiteSpace(PlyFilePath));
        }

        private void ReadPlyCsv()
        {
            _dataService.GetPlyData(
                (items, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        _ioService.Message(error.Message, Properties.Resources.Error);
                        return;
                    }

                    PlyFiles = new ObservableCollection<PlyFile>(items);

                }, _plyFilePath.Trim('"'), _delimiter, _headers ? 1 : 0);
        }

        private void GetPlyFilePath()
        {
            var defaultPath = string.IsNullOrWhiteSpace(Properties.Settings.Default.InitialFolder)
                ? _appFilePath
                : Properties.Settings.Default.InitialFolder;

            PlyFilePath = _ioService.OpenFileDialog(defaultPath, CsvFiles);
            
            if(!string.IsNullOrWhiteSpace(PlyFilePath)) ReadPlyCsv();
        }

        private async void CreatePlies()
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

            await Task.Run( () =>_ioService.CreatePlyDxf((error) =>
            {
                if (error != null)
                {
                    _ioService.Message(error.Message, Properties.Resources.Error);
                }

                _ioService.Message(Properties.Resources.Success + _savePath, Properties.Resources.FileOperation);
            }, PlyFiles.ToList(), _savePath));

            PlyFiles.Clear();
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
                _plyFilePath = dragFileList.First();
                ReadPlyCsv();
            }

        }
    }
}
