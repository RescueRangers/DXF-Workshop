using System;
using System.Collections.Generic;
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
    public class PlxTabViewModel : ViewModelBase, IDropTarget
    {
        private readonly IIOService _ioService;
        private PlxOptions _plxOptions = new PlxOptions();
        private string _savePath;
        private PlxFile _plxFile;
        private string _plxFileName;

        public PlxOptions PlxOptions
        {
            get => _plxOptions;
            set => Set(ref _plxOptions, value);
        }

        public string PlxFileName
        {
            get => _plxFileName;
            set => Set(ref _plxFileName, value);
        }

        public PlxFile PlxFile
        {
            get => _plxFile;
            set => Set(ref _plxFile, value);
        }

        public ICommand CreatePlxCommand { get; private set; }
        public ICommand GetFilesCommand { get; private set; }
        public string SavePath { get => _savePath; set => _savePath = value; }

        public PlxTabViewModel(IIOService ioService)
        {
            _ioService = ioService;
            LoadCommands();

        }

        private void LoadCommands()
        {
            CreatePlxCommand = new RelayCommand(CreatePlx,
                            () => PlxFile != null && PlxFile.DxfFiles.Any() && !string.IsNullOrWhiteSpace(PlxFileName));
            GetFilesCommand = new RelayCommand(GetFiles, () => true);
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
                    SavePath = savePath;
                });

            if (dxfFileNames == null) return;

            PlxFile = new PlxFile(dxfFileNames);
        }

        private void CreatePlx()
        {
            var fileContents = PlxFile.CreateFile(_plxFileName, _plxOptions);
            var fullSavePath = $"{SavePath}\\{PlxFileName}.plx";

            File.AppendAllLines(fullSavePath, fileContents);

            _ioService.Message(Properties.Resources.FileSavedMessage + $"{SavePath}", Properties.Resources.FileSavedTitle);
            ClearData();

        }

        private void ClearData()
        {
            PlxFile = null;
            SavePath = null;
            PlxFileName = null;
        }

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

                SavePath = directory;
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

                SavePath = directory;
            }, fileList);
        }
    }
}
