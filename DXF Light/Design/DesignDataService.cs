using System;
using System.Collections.Generic;
using DXF_Light.Model;

namespace DXF_Light.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<List<DxfFile>, Exception> callback, string filePath, string delimiter, int headers)
        {
            // Use this to create design time data

            var item = new DxfFile()
            {
                Length = 1500,
                Width = 1270,
                Name = "Ply-01"
            };

            var dxfs = new List<DxfFile>{item};

            callback(dxfs, null);
        }

    }
}