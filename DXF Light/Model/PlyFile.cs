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
        [FieldConverter(ConverterKind.Decimal, ",")]
        public decimal W { get; set; }
        [FieldConverter(ConverterKind.Decimal, ",")]
        public decimal L1 { get; set; }
        [FieldConverter(ConverterKind.Decimal, ",")]
        public decimal L2 { get; set; }
        [FieldConverter(ConverterKind.Decimal, ",")]
        public decimal L3 { get; set; }
        [FieldConverter(ConverterKind.Decimal, ",")]
        public decimal L4 { get; set; }

        [FieldOptional]
        public string Material { get; set; }
    }
}
