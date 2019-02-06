using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace DXF_Light.Model
{
    public class DataService : IDataService
    {
        public void GetData(Action<List<DxfFile>, Exception> callback, string filePath, string delimiter, int headers)
        {
            // Use this to connect to the actual data service

            try
            {
                var engine = new DelimitedFileEngine<DxfFile>(Encoding.Default);
                engine.Options.Delimiter = delimiter;
                engine.Options.IgnoreFirstLines = headers;
                var result = engine.ReadFile(filePath).ToList();
                callback(result, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                callback(null, e);
            }
        }

        public void GetPlyData(Action<List<PlyFile>, Exception> callback, string filePath, string delimiter, int headers)
        {
            // Use this to connect to the actual data service

            try
            {
                var engine = new DelimitedFileEngine<PlyFile>(Encoding.Default);
                engine.Options.Delimiter = delimiter;
                engine.Options.IgnoreFirstLines = headers;
                var result = engine.ReadFile(filePath).ToList();
                callback(result, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                callback(null, e);
            }
        }

        public void GetNCDxfData(Action<List<InternalCut>, Exception> callback, string filePath, string delimiter, int headers)
        {
            try
            {
                var engine = new DelimitedFileEngine<InternalCut>(Encoding.Default);
                engine.Options.Delimiter = delimiter;
                engine.Options.IgnoreFirstLines = headers;
                var result = engine.ReadFile(filePath).ToList();
                callback(result, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                callback(null, e);
            }
        }
    }
}