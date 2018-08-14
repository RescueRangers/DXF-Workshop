using FileHelpers;

namespace DXF_Light.Model
{
    [DelimitedRecord(";")]
    public class DxfFile
    {
        public string Name { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        [FieldOptional]
        public string Material { get; set; }

        [FieldOptional] [FieldNullValue(1)] public int Qty { get; set; } = 1;
        [FieldOptional] [FieldNullValue(1)] public int Group { get; set; } = 1;
    }
}
