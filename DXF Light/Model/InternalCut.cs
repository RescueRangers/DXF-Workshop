using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace DXF_Light.Model
{
    [DelimitedRecord(";")]
    public class InternalCut
    {
        [FieldOrder(1)]
        [FieldConverter(ConverterKind.Double, ",")]
        public double Cut { get; set; }

        [FieldOrder(2)]
        [FieldOptional]
        public string[] TheRest;

        public InternalCut()
        {
            
        }

        public InternalCut(double  cut)
        {
            Cut = cut;
        }

    }

}
