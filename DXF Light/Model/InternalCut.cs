using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXF_Light.Model
{
    public class InternalCut
    {
        public decimal Cut { get; set; }

        public InternalCut()
        {
            
        }

        public InternalCut(decimal  cut)
        {
            Cut = cut;
        }

    }

}
