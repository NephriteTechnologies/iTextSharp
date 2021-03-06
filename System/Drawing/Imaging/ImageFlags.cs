using System;

namespace iTextSharp.Drawing.Imaging
{
	[Flags]
	public enum ImageFlags
	{
		Caching = 131072,
		ColorSpaceCmyk = 32,
		ColorSpaceGray = 64,
		ColorSpaceRgb = 16,
		ColorSpaceYcbcr = 128,
		ColorSpaceYcck = 256,
		HasAlpha = 2,
		HasRealDpi = 4096,
		HasRealPixelSize = 8192,
		HasTranslucent = 4,
		None = 0,
		PartiallyScalable = 8,
		ReadOnly = 65536,
		Scalable = 1
	}
}
