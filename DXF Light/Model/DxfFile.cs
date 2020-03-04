using FileHelpers;

namespace DXF_Light.Model
{
    [DelimitedRecord(";")]
    public class DxfFile
    {
        [FieldOrder(1)]
        public string Name { get; set; }

        [FieldOrder(2)]
        [FieldConverter(ConverterKind.Double, ",")]
        public double Width { get; set; }

        [FieldOrder(3)]
        [FieldConverter(ConverterKind.Double, ",")]
        public double Length { get; set; }

        [FieldOrder(4)]
        [FieldOptional]
        public string Material { get; set; }

        [FieldOrder(5)] [FieldOptional] [FieldNullValue(1)] public int Qty { get; set; } = 1;
        [FieldOrder(6)] [FieldOptional] [FieldNullValue(1)] public int Group { get; set; } = 1;

        [FieldOrder(7)]
        [FieldOptional]
        public string[] TheRest;
    }
}