using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DXF_Light.Model
{
	public class PlxOptions : ObservableObject
	{
		public delegate void PlxOptionsUpdateHandler(object sender, EventArgs e);
		public event PlxOptionsUpdateHandler OnPlxOptionsUpdate;

		public int Width { get; set; } = 1000;
		public int Length { get; set; } = 100000;
		public bool OnePlx
		{
			get => onePlx; 
			set
			{
				SetProperty(ref onePlx, value);
				OnPlxOptionsUpdate(this, null);
			}
		}
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
		private bool onePlx;
	}
}