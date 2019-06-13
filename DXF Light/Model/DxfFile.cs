using FileHelpers;
using NCalc;

namespace DXF_Light.Model
{
    [DelimitedRecord(";")]
    public class DxfFile
    {
        [FieldOrder(1)]
        public string Name { get; set; }

        [FieldHidden]
        public string Width
        {
            get { return _width; }
            set
            {
                var e = new Expression(value);
                _width = e.Evaluate().ToString();
            }
        }

        [FieldHidden]
        public string Length
        {
            get { return _length; }
            set
            {
                var e = new Expression(value);
                _length = e.Evaluate().ToString();
            }
        }

        [FieldOrder(4)]
        [FieldOptional]
        public string Material { get; set; }

        [FieldOrder(5)]
        [FieldOptional]
        [FieldNullValue(1)]
        public int Qty { get; set; } = 1;

        [FieldOrder(6)]
        [FieldOptional]
        [FieldNullValue(1)]
        public int Group { get; set; } = 1;

        [FieldOrder(7)]
        [FieldOptional]
        public string[] TheRest;

        [FieldOrder(2)]
        private string _width;

        [FieldOrder(3)]
        private string _length;
    }
}
