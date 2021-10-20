namespace DXF_Light.Model
{
    public class PlxOptions
    {
        public int Width { get; set; } = 1000;
        public int Length { get; set; } = 100000;
        public bool OnePlx { get; set; }
        public bool SameWidth { get; set; }

        public bool PatchesNesting
        {
            get => _patchesNesting;
            set
            {
                _patchesNesting = value;
                if (value)
                {
                    OnePlx = false;
                    SameWidth = false;
                }
            }
        }

        public bool RealizeAscending { get; set; } = true;

        private bool _patchesNesting;
    }
}