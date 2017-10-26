using System;

namespace iTextSharp.Drawing.Printing
{
	[Serializable]
	public class PageSettings : ICloneable
	{
		internal bool color;

		internal bool landscape;

		internal PaperSize paperSize;

		internal PaperSource paperSource;

		internal PrinterResolution printerResolution;

		private Margins margins;

		private float hardMarginX;

		private float hardMarginY;

		private RectangleF printableArea;

		private PrinterSettings printerSettings;

		public Rectangle Bounds
		{
			get
			{
				int num = this.paperSize.Width;
				int num2 = this.paperSize.Height;
				num -= this.margins.Left + this.margins.Right;
				num2 -= this.margins.Top + this.margins.Bottom;
				if (this.landscape)
				{
					int arg_57_0 = num;
					num = num2;
					num2 = arg_57_0;
				}
				return new Rectangle(this.margins.Left, this.margins.Top, num, num2);
			}
		}

		public bool Color
		{
			get
			{
				if (!this.printerSettings.IsValid)
				{
					throw new InvalidPrinterException(this.printerSettings);
				}
				return this.color;
			}
			set
			{
				this.color = value;
			}
		}

		public bool Landscape
		{
			get
			{
				if (!this.printerSettings.IsValid)
				{
					throw new InvalidPrinterException(this.printerSettings);
				}
				return this.landscape;
			}
			set
			{
				this.landscape = value;
			}
		}

		public Margins Margins
		{
			get
			{
				if (!this.printerSettings.IsValid)
				{
					throw new InvalidPrinterException(this.printerSettings);
				}
				return this.margins;
			}
			set
			{
				this.margins = value;
			}
		}

		public PaperSize PaperSize
		{
			get
			{
				if (!this.printerSettings.IsValid)
				{
					throw new InvalidPrinterException(this.printerSettings);
				}
				return this.paperSize;
			}
			set
			{
				if (value != null)
				{
					this.paperSize = value;
				}
			}
		}

		public PaperSource PaperSource
		{
			get
			{
				if (!this.printerSettings.IsValid)
				{
					throw new InvalidPrinterException(this.printerSettings);
				}
				return this.paperSource;
			}
			set
			{
				if (value != null)
				{
					this.paperSource = value;
				}
			}
		}

		public PrinterResolution PrinterResolution
		{
			get
			{
				if (!this.printerSettings.IsValid)
				{
					throw new InvalidPrinterException(this.printerSettings);
				}
				return this.printerResolution;
			}
			set
			{
				if (value != null)
				{
					this.printerResolution = value;
				}
			}
		}

		public PrinterSettings PrinterSettings
		{
			get
			{
				return this.printerSettings;
			}
			set
			{
				this.printerSettings = value;
			}
		}

		public float HardMarginX
		{
			get
			{
				return this.hardMarginX;
			}
		}

		public float HardMarginY
		{
			get
			{
				return this.hardMarginY;
			}
		}

		public RectangleF PrintableArea
		{
			get
			{
				return this.printableArea;
			}
		}

		public PageSettings() : this(new PrinterSettings())
		{
		}

		public PageSettings(PrinterSettings printerSettings)
		{
			this.margins = new Margins();
			//base..ctor();
			this.PrinterSettings = printerSettings;
			this.color = printerSettings.DefaultPageSettings.color;
			this.landscape = printerSettings.DefaultPageSettings.landscape;
			this.paperSize = printerSettings.DefaultPageSettings.paperSize;
			this.paperSource = printerSettings.DefaultPageSettings.paperSource;
			this.printerResolution = printerSettings.DefaultPageSettings.printerResolution;
		}

		internal PageSettings(PrinterSettings printerSettings, bool color, bool landscape, PaperSize paperSize, PaperSource paperSource, PrinterResolution printerResolution)
		{
			this.margins = new Margins();
			//base..ctor();
			this.PrinterSettings = printerSettings;
			this.color = color;
			this.landscape = landscape;
			this.paperSize = paperSize;
			this.paperSource = paperSource;
			this.printerResolution = printerResolution;
		}

		public object Clone()
		{
			PrinterResolution printerResolution = new PrinterResolution(this.printerResolution.X, this.printerResolution.Y, this.printerResolution.Kind);
			PaperSource paperSource = new PaperSource(this.paperSource.SourceName, this.paperSource.Kind);
			PaperSize paperSize = new PaperSize(this.paperSize.PaperName, this.paperSize.Width, this.paperSize.Height);
			paperSize.SetKind(this.paperSize.Kind);
			return new PageSettings(this.printerSettings, this.color, this.landscape, paperSize, paperSource, printerResolution)
			{
				Margins = (Margins)this.margins.Clone()
			};
		}

		[MonoTODO("PageSettings.CopyToHdevmode")]
		public void CopyToHdevmode(IntPtr hdevmode)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("PageSettings.SetHdevmode")]
		public void SetHdevmode(IntPtr hdevmode)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return string.Format("[PageSettings: Color={0}" + ", Landscape={1}" + ", Margins={2}" + ", PaperSize={3}" + ", PaperSource={4}" + ", PrinterResolution={5}" + "]", new object[]
			{
				this.color,
				this.landscape,
				this.margins,
				this.paperSize,
				this.paperSource,
				this.printerResolution
			});
		}
	}
}
