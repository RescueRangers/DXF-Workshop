using System;
using System.Collections.Generic;

namespace DXF_Light.Model
{
    public interface IDataService
    {
        void GetData(Action<List<DxfFile>, Exception> callback, string filePath, string delimiter, int headers);
        void GetPlyData(Action<List<PlyFile>, Exception> callback, string filePath, string delimiter, int headers);
    }
}
