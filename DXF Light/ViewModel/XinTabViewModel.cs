﻿using DXF_Light.Model;
using DXF_Light.Servicess;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GongSolutions.Wpf.DragDrop;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DXF_Light.ViewModel
{
    public class XinTabViewModel : ViewModelBase, IDropTarget
    {
        private readonly IDataService _dataService;
        private readonly IIOService _ioService;
        private ObservableCollection<DxfFile> _dxfFiles = new ObservableCollection<DxfFile>();
        private string _savePath;

        public string SavePath { get => _savePath; set => _savePath = value; }

        public ObservableCollection<DxfFile> DxfFiles 
        {
            get => _dxfFiles; 
            set => Set(nameof(DxfFiles), ref _dxfFiles, value); 
        }

        public ICommand GetDxfFiles{ get; set; }
        public ICommand CreateXinFiles { get; set; }

        public XinTabViewModel(IDataService dataService, IIOService iOService)
        {
            _dataService = dataService;
            _ioService = iOService;

            GetDxfFiles = new RelayCommand(GetFiles);
            CreateXinFiles = new RelayCommand(CreateXin, () => DxfFiles.Any());
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

        private void CreateXin()
        {
            foreach (var item in DxfFiles)
            {
                var fileContents = XinFileBuilder.CreateXin(item.Name);
                var fullSavePath = $"{SavePath}\\{item.Name}.xin";

                File.AppendAllLines(fullSavePath, fileContents);
            }

            DxfFiles.Clear();
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

            DxfFiles.AddRange(dxfFileNames);
        }

        private void AddDxfsFromPath(List<string> dragFileList)
        {
            _ioService.GetDxfFilesFromPaths((dxfFiles, directory, exception) =>
            {
                if (exception != null)
                {
                    _ioService.Message(exception.Message, Properties.Resources.Error);
                    return;
                }

                DxfFiles.AddRange(dxfFiles);

                SavePath = directory;
            }, dragFileList);
        }
    }
}