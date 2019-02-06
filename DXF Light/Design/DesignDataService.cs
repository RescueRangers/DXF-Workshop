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

        public void GetPlyData(Action<List<PlyFile>, Exception> callback, string filePath, string delimiter, int headers)
        {
            var item = new PlyFile()
            {
                L1 = 1500,
                L2 = 1270,
                L3 = 1278,
                L4 = 260,
                W = 1260,
                Name = "Ply-01"
            };

            var plies = new List<PlyFile>{item};

            callback(plies, null);
        }

        public void GetNCDxfData(Action<List<InternalCut>, Exception> callback, string filePath, string delimiter, int headers)
        {
            var cuts = new List<InternalCut>{new InternalCut(714), new InternalCut(714), new InternalCut(851)};
            callback(cuts, null);
        }
    }
}