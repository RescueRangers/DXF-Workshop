using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace DXF_Light.Model
{
    [DelimitedRecord(";")]
    public class PlyFile
    {
        public string Name { get; set; }
        public decimal W { get; set; }
        public decimal L1 { get; set; }
        public decimal L2 { get; set; }
        public decimal L3 { get; set; }
        public decimal L4 { get; set; }

        [FieldOptional]
        public string Material { get; set; }
    }
}
