using System.Windows.Input;
using GalaSoft.MvvmLight;
using DXF_Light.Model;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using DXF_Light.Servicess;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Practices.ServiceLocation;
using NuGet;
using WPFLocalizeExtension.Engine;

namespace DXF_Light.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, IDropTarget
    {
        private readonly IDataService _dataService;
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

        private readonly string _appFilePath = AppDomain.CurrentDomain.BaseDirectory;
        private const string CsvFiles = "Pliki csv (.csv)|*.csv";

        private string _filePath;
        private string _delimiter = ";";
        private bool _headers;
        private string _savePath;
        private string _plxFileName;
        private PlxFile _plxFile;
        private PlxOptions _plxOptions;
        private ObservableCollection<DxfFile> _dxfFiles;
        private static string _startupArgument;

        public PlxOptions PlxOptions
        {
            get => _plxOptions;
            set => Set(ref _plxOptions, value);
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

        public PlxFile PlxFile
        {
            get => _plxFile;
            set => Set(ref _plxFile, value);
        }

        public ObservableCollection<DxfFile> DxfFiles
        {
            get => _dxfFiles;
            set => Set(ref _dxfFiles, value);
        }

        public string PlxFileName
        {
            get => _plxFileName;
            set => Set(ref _plxFileName, value);
        }

        public ICommand GetFilePathCommand { get; private set; }
        public ICommand ReadFileCommand { get; private set; }
        public ICommand CreateDxfsCommand { get; private set; }
        public ICommand GetFilesCommand { get; private set; }
        public ICommand CreatePlxCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }
        public ICommand EnglishCommand { get; private set; }
        public ICommand PolishCommand { get; private set; }
        public ICommand AddDxfCommand { get; private set; }
        public ICommand LoadedCommand { get; private set; }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _ioService = new IOService();
            _dataService = dataService;
            PlxOptions = new PlxOptions();
            DxfFiles = new ObservableCollection<DxfFile>();
            LoadCommands();
        }

        private void LoadCommands()
        {
            GetFilePathCommand = new RelayCommand(GetFilePath, () => true);
            ReadFileCommand = new RelayCommand(ReadCsv, () => !string.IsNullOrWhiteSpace(FilePath));
            CreateDxfsCommand = new RelayCommand(CreateDxfs, () => DxfFiles != null && DxfFiles.Count > 0);
            GetFilesCommand = new RelayCommand(GetFiles, () => true);
            CreatePlxCommand = new RelayCommand(CreatePlx,
                () => PlxFile != null && PlxFile.DxfFiles.Any() && !string.IsNullOrWhiteSpace(PlxFileName));
            ExitCommand = new RelayCommand(Exit, () => true);
            PolishCommand = new RelayCommand(Polish, () => true);
            EnglishCommand = new RelayCommand(English, () => true);
            AddDxfCommand = new RelayCommand(AddDxf, () => true);
            LoadedCommand = new RelayCommand(Loaded, () => true);
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

                    PlxFile = new PlxFile(dxfFiles);
                    _savePath = directory;
                }, _arguments);
            }
        }

        private void AddDxf()
        {
            DxfFiles.Add(new DxfFile());
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

        private void CreatePlx()
        {
            var fileContents = PlxFile.CreateFile(_plxFileName, _plxOptions);
            var fullSavePath = $"{_savePath}\\{PlxFileName}.plx";

            File.AppendAllLines(fullSavePath, fileContents);

            _ioService.Message(Properties.Resources.FileSavedMessage + $"{_savePath}", Properties.Resources.FileSavedTitle);
            ClearData();

        }

        private void ClearData()
        {
            PlxFile = null;
            _savePath = null;
            PlxFileName = null;
            _dxfFiles = null;
            FilePath = null;
        }

        private void GetFiles()
        {
            var dxfFileNames = new List<DxfFile>();
            _ioService.GetDxfFiles(
                (item, savePath, error) =>
                {
                    if (error != null)
                    {
                        _ioService.Message(error.Message, Properties.Resources.Error);
                        dxfFileNames = null;
                        return;
                    }

                    dxfFileNames = item;
                    _savePath = savePath;
                });

            if (dxfFileNames == null) return;

            PlxFile = new PlxFile(dxfFileNames);
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

            await Task.Run( () =>_ioService.CreateDxfFiles((error) =>
            {
                if (error != null)
                {
                    _ioService.Message(error.Message, Properties.Resources.Error);
                }

                _ioService.Message(Properties.Resources.Success + _savePath, Properties.Resources.FileOperation);
            }, DxfFiles.ToList(), _savePath));
            
            
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

                }, _filePath, _delimiter, _headers ? 1 : 0);
        }

        private void GetFilePath()
        {
            FilePath = _ioService.OpenFileDialog(_appFilePath, CsvFiles);
            
            if(!string.IsNullOrWhiteSpace(FilePath)) ReadCsv();
        }

        

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
        public void DragOver(IDropInfo dropInfo)
        {
            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = dragFileList.Any(item =>
            {
                var extension = Path.GetExtension(item);
                return extension != null && extension.Equals(".dxf");
            }) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        public void Drop(IDropInfo dropInfo)
        {
            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>().ToList();

            if (dragFileList.Any(item =>
            {
                var extension = Path.GetExtension(item);
                return extension != null && extension.Equals(".dxf");
            }))
            {
                AddDxfsFromPath(dragFileList);
            }

        }

        private void AddDxfsFromPath(List<string> fileList)
        {

            _ioService.GetDxfFilesFromPaths((dxfFiles, directory, exception) =>
            {
                if (exception != null)
                {
                    _ioService.Message(exception.Message, Properties.Resources.Error);
                    return;
                }

                if (PlxFile == null)
                {
                    PlxFile = new PlxFile(dxfFiles);
                }
                else
                {
                    PlxFile.AddDxfToFile(dxfFiles);
                }

                _savePath = directory;
            }, fileList);
        }

        private void AddDxfsFromPath(string file)
        {
            var fileList = new List<string> {file};

            _ioService.GetDxfFilesFromPaths((dxfFiles, directory, exception) =>
            {
                if (exception != null)
                {
                    _ioService.Message(exception.Message, Properties.Resources.Error);
                    return;
                }

                if (PlxFile == null)
                {
                    PlxFile = new PlxFile(dxfFiles);
                }
                else
                {
                    PlxFile.AddDxfToFile(dxfFiles);
                }

                _savePath = directory;
            }, fileList);
        }
    }
}