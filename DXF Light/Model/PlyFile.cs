using FileHelpers;

namespace DXF_Light.Model
{
    [DelimitedRecord(";")]
    public class PlyFile
    {
        [FieldOrder(1)]
        public string Name { get; set; }

        [FieldOrder(2)]
        [FieldConverter(ConverterKind.Double, ",")]
        public double W { get; set; }

        [FieldOrder(3)]
        [FieldConverter(ConverterKind.Double, ",")]
        public double L1 { get; set; }

        [FieldOrder(4)]
        [FieldConverter(ConverterKind.Double, ",")]
        public double L2 { get; set; }

        [FieldOrder(5)]
        [FieldConverter(ConverterKind.Double, ",")]
        public double L3 { get; set; }

        [FieldOrder(6)]
        [FieldConverter(ConverterKind.Double, ",")]
        public double L4 { get; set; }

        [FieldOrder(7)]
        [FieldOptional]
        public string Material { get; set; }

        [FieldOrder(8)]
        [FieldOptional]
        public string[] TheRest;
    }
}