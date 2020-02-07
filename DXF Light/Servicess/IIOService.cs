using System;
using System.Collections.Generic;
using DXF_Light.Model;

namespace DXF_Light.Servicess
{
    public interface IIOService
    {
        string OpenFileDialog(string defaultPath, string fileFilter);

        void Message(string messageText, string title);

        void GetFolder(Action<string, Exception> callback);

        void GetDxfFiles(Action<List<DxfFile>, string, Exception> callback);

        void GetDxfFilesFromPaths(Action<List<DxfFile>, string, Exception> callback, List<string> paths);

        void CreatePlx(Action<Exception> callback, PlxFile plxFile, string path);

        void CreateDxfFiles(Action<Exception> callback, List<DxfFile> dxfFiles, string path);

        void CreatePlyDxf(Action<Exception> callback, List<PlyFile> plyFiles, string path);

        void CreateNCDxf(Action<Exception> callback, NoContourDxf nCDxf, string path);
    }
}