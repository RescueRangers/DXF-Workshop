using FileHelpers;
using NCalc;

namespace DXF_Light.Model
{
    [DelimitedRecord(";")]
    public class DxfFile
    {

        public DxfFile()
        {
            
        }
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
        public decimal WidthValue { 
            get 
            { 
                var result = decimal.TryParse(Width, out var e);
                if( result) return e;
                return 0;
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
		
        [FieldHidden]
		public decimal LengthValue
		{
			get
			{
				var result = decimal.TryParse(Length, out var e);
				if (result) return e;
				return 0;
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