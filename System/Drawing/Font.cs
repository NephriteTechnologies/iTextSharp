using System;
using System.ComponentModel;
using iTextSharp.Drawing.Design;
using iTextSharp.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace iTextSharp.Drawing
{
	[Editor("System.Drawing.Design.FontEditor, System.Drawing.Design", typeof(UITypeEditor)), TypeConverter(typeof(FontConverter)), ComVisible(true)]
	[Serializable]
	public sealed class Font : MarshalByRefObject, ISerializable, ICloneable, IDisposable
	{
		private IntPtr fontObject = IntPtr.Zero;

		private string systemFontName;

		private string originalFontName;

		private float _size;

		private object olf;

		private const byte DefaultCharSet = 1;

		private static int CharSetOffset = -1;

		private bool _bold;

		private FontFamily _fontFamily;

		private byte _gdiCharSet;

		private bool _gdiVerticalFont;

		private bool _italic;

		private string _name;

		private float _sizeInPoints;

		private bool _strikeout;

		private FontStyle _style;

		private bool _underline;

		private GraphicsUnit _unit;

		private int _hashCode;

		internal IntPtr NativeObject
		{
			get
			{
				return this.fontObject;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Bold
		{
			get
			{
				return this._bold;
			}
		}

		[Browsable(false)]
		public FontFamily FontFamily
		{
			get
			{
				return this._fontFamily;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public byte GdiCharSet
		{
			get
			{
				return this._gdiCharSet;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool GdiVerticalFont
		{
			get
			{
				return this._gdiVerticalFont;
			}
		}

		[Browsable(false)]
		public int Height
		{
			get
			{
				return (int)Math.Ceiling((double)this.GetHeight());
			}
		}

		[Browsable(false)]
		public bool IsSystemFont
		{
			get
			{
				return !string.IsNullOrEmpty(this.systemFontName);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Italic
		{
			get
			{
				return this._italic;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Editor("System.Drawing.Design.FontNameEditor, System.Drawing.Design", typeof(UITypeEditor)), TypeConverter(typeof(FontConverter.FontNameConverter))]
		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public float Size
		{
			get
			{
				return this._size;
			}
		}

		[Browsable(false)]
		public float SizeInPoints
		{
			get
			{
				return this._sizeInPoints;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Strikeout
		{
			get
			{
				return this._strikeout;
			}
		}

		[Browsable(false)]
		public FontStyle Style
		{
			get
			{
				return this._style;
			}
		}

		[Browsable(false)]
		public string SystemFontName
		{
			get
			{
				return this.systemFontName;
			}
		}

		[Browsable(false)]
		public string OriginalFontName
		{
			get
			{
				return this.originalFontName;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Underline
		{
			get
			{
				return this._underline;
			}
		}

		[TypeConverter(typeof(FontConverter.FontUnitConverter))]
		public GraphicsUnit Unit
		{
			get
			{
				return this._unit;
			}
		}

		private void CreateFont(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte charSet, bool isVertical)
		{
			this.originalFontName = familyName;
			FontFamily fontFamily;
			try
			{
				fontFamily = new FontFamily(familyName);
			}
			catch (Exception)
			{
				fontFamily = FontFamily.GenericSansSerif;
			}
			this.setProperties(fontFamily, emSize, style, unit, charSet, isVertical);
			Status status = GDIPlus.GdipCreateFont(fontFamily.NativeObject, emSize, style, unit, out this.fontObject);
			if (status == Status.FontStyleNotFound)
			{
				throw new ArgumentException(Locale.GetText("Style {0} isn't supported by font {1}.", new object[]
				{
					style.ToString(),
					familyName
				}));
			}
			GDIPlus.CheckStatus(status);
		}

		private Font(SerializationInfo info, StreamingContext context)
		{
			string familyName = (string)info.GetValue("Name", typeof(string));
			float emSize = (float)info.GetValue("Size", typeof(float));
			FontStyle style = (FontStyle)info.GetValue("Style", typeof(FontStyle));
			GraphicsUnit unit = (GraphicsUnit)info.GetValue("Unit", typeof(GraphicsUnit));
			this.CreateFont(familyName, emSize, style, unit, 1, false);
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			si.AddValue("Name", this.Name);
			si.AddValue("Size", this.Size);
			si.AddValue("Style", this.Style);
			si.AddValue("Unit", this.Unit);
		}

		~Font()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			if (this.fontObject != IntPtr.Zero)
			{
				Status arg_2E_0 = GDIPlus.GdipDeleteFont(this.fontObject);
				this.fontObject = IntPtr.Zero;
				GC.SuppressFinalize(this);
				GDIPlus.CheckStatus(arg_2E_0);
			}
		}

		internal void unitConversion(GraphicsUnit fromUnit, GraphicsUnit toUnit, float nSrc, out float nTrg)
		{
			nTrg = 0f;
			float num;
			switch (fromUnit)
			{
			case GraphicsUnit.World:
			case GraphicsUnit.Pixel:
				num = nSrc / Graphics.systemDpiX;
				break;
			case GraphicsUnit.Display:
				num = nSrc / 75f;
				break;
			case GraphicsUnit.Point:
				num = nSrc / 72f;
				break;
			case GraphicsUnit.Inch:
				num = nSrc;
				break;
			case GraphicsUnit.Document:
				num = nSrc / 300f;
				break;
			case GraphicsUnit.Millimeter:
				num = nSrc / 25.4f;
				break;
			default:
				throw new ArgumentException("Invalid GraphicsUnit");
			}
			switch (toUnit)
			{
			case GraphicsUnit.World:
			case GraphicsUnit.Pixel:
				nTrg = num * Graphics.systemDpiX;
				return;
			case GraphicsUnit.Display:
				nTrg = num * 75f;
				return;
			case GraphicsUnit.Point:
				nTrg = num * 72f;
				return;
			case GraphicsUnit.Inch:
				nTrg = num;
				return;
			case GraphicsUnit.Document:
				nTrg = num * 300f;
				return;
			case GraphicsUnit.Millimeter:
				nTrg = num * 25.4f;
				return;
			default:
				throw new ArgumentException("Invalid GraphicsUnit");
			}
		}

		private void setProperties(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte charSet, bool isVertical)
		{
			this._name = family.Name;
			this._fontFamily = family;
			this._size = emSize;
			this._unit = unit;
			this._style = style;
			this._gdiCharSet = charSet;
			this._gdiVerticalFont = isVertical;
			this.unitConversion(unit, GraphicsUnit.Point, emSize, out this._sizeInPoints);
			this._bold = (this._italic = (this._strikeout = (this._underline = false)));
			if ((style & FontStyle.Bold) == FontStyle.Bold)
			{
				this._bold = true;
			}
			if ((style & FontStyle.Italic) == FontStyle.Italic)
			{
				this._italic = true;
			}
			if ((style & FontStyle.Strikeout) == FontStyle.Strikeout)
			{
				this._strikeout = true;
			}
			if ((style & FontStyle.Underline) == FontStyle.Underline)
			{
				this._underline = true;
			}
		}

		public static Font FromHfont(IntPtr hfont)
		{
			FontStyle fontStyle = FontStyle.Regular;
			LOGFONT lOGFONT = default(LOGFONT);
			if (hfont == IntPtr.Zero)
			{
				return new Font("Arial", 10f, FontStyle.Regular);
			}
			IntPtr newFontObject;
			if (GDIPlus.RunningOnUnix())
			{
				GDIPlus.CheckStatus(GDIPlus.GdipCreateFontFromHfont(hfont, out newFontObject, ref lOGFONT));
			}
			else
			{
				IntPtr dC = GDIPlus.GetDC(IntPtr.Zero);
				try
				{
					return Font.FromLogFont(lOGFONT, dC);
				}
				finally
				{
					GDIPlus.ReleaseDC(IntPtr.Zero, dC);
				}
			}
			if (lOGFONT.lfItalic != 0)
			{
				fontStyle |= FontStyle.Italic;
			}
			if (lOGFONT.lfUnderline != 0)
			{
				fontStyle |= FontStyle.Underline;
			}
			if (lOGFONT.lfStrikeOut != 0)
			{
				fontStyle |= FontStyle.Strikeout;
			}
			if (lOGFONT.lfWeight > 400u)
			{
				fontStyle |= FontStyle.Bold;
			}
			float size;
			if (lOGFONT.lfHeight < 0)
			{
				size = (float)(lOGFONT.lfHeight * -1);
			}
			else
			{
				size = (float)lOGFONT.lfHeight;
			}
			return new Font(newFontObject, lOGFONT.lfFaceName, fontStyle, size);
		}

		public IntPtr ToHfont()
		{
			if (this.fontObject == IntPtr.Zero)
			{
				throw new ArgumentException(Locale.GetText("Object has been disposed."));
			}
			if (GDIPlus.RunningOnUnix())
			{
				return this.fontObject;
			}
			if (this.olf == null)
			{
				this.olf = default(LOGFONT);
				this.ToLogFont(this.olf);
			}
			LOGFONT lOGFONT = (LOGFONT)this.olf;
			return GDIPlus.CreateFontIndirect(ref lOGFONT);
		}

		internal Font(IntPtr newFontObject, string familyName, FontStyle style, float size)
		{
			FontFamily family;
			try
			{
				family = new FontFamily(familyName);
			}
			catch (Exception)
			{
				family = FontFamily.GenericSansSerif;
			}
			this.setProperties(family, size, style, GraphicsUnit.Pixel, 0, false);
			this.fontObject = newFontObject;
		}

		public Font(Font prototype, FontStyle newStyle)
		{
			this.setProperties(prototype.FontFamily, prototype.Size, newStyle, prototype.Unit, prototype.GdiCharSet, prototype.GdiVerticalFont);
			GDIPlus.CheckStatus(GDIPlus.GdipCreateFont(this._fontFamily.NativeObject, this.Size, this.Style, this.Unit, out this.fontObject));
		}

		public Font(FontFamily family, float emSize, GraphicsUnit unit) : this(family, emSize, FontStyle.Regular, unit, 1, false)
		{
		}

		public Font(string familyName, float emSize, GraphicsUnit unit) : this(new FontFamily(familyName), emSize, FontStyle.Regular, unit, 1, false)
		{
		}

		public Font(FontFamily family, float emSize) : this(family, emSize, FontStyle.Regular, GraphicsUnit.Point, 1, false)
		{
		}

		public Font(FontFamily family, float emSize, FontStyle style) : this(family, emSize, style, GraphicsUnit.Point, 1, false)
		{
		}

		public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit) : this(family, emSize, style, unit, 1, false)
		{
		}

		public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet) : this(family, emSize, style, unit, gdiCharSet, false)
		{
		}

		public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
		{
			if (family == null)
			{
				throw new ArgumentNullException("family");
			}
			this.setProperties(family, emSize, style, unit, gdiCharSet, gdiVerticalFont);
			GDIPlus.CheckStatus(GDIPlus.GdipCreateFont(family.NativeObject, emSize, style, unit, out this.fontObject));
		}

		public Font(string familyName, float emSize) : this(familyName, emSize, FontStyle.Regular, GraphicsUnit.Point, 1, false)
		{
		}

		public Font(string familyName, float emSize, FontStyle style) : this(familyName, emSize, style, GraphicsUnit.Point, 1, false)
		{
		}

		public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit) : this(familyName, emSize, style, unit, 1, false)
		{
		}

		public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet) : this(familyName, emSize, style, unit, gdiCharSet, false)
		{
		}

		public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
		{
			this.CreateFont(familyName, emSize, style, unit, gdiCharSet, gdiVerticalFont);
		}

		internal Font(string familyName, float emSize, string systemName) : this(familyName, emSize, FontStyle.Regular, GraphicsUnit.Point, 1, false)
		{
			this.systemFontName = systemName;
		}

		public object Clone()
		{
			return new Font(this, this.Style);
		}

		public override bool Equals(object obj)
		{
			Font font = obj as Font;
			return font != null && (font.FontFamily.Equals(this.FontFamily) && font.Size == this.Size && font.Style == this.Style && font.Unit == this.Unit && font.GdiCharSet == this.GdiCharSet && font.GdiVerticalFont == this.GdiVerticalFont);
		}

		public override int GetHashCode()
		{
			if (this._hashCode == 0)
			{
				this._hashCode = 17;
				this._hashCode = this._hashCode * 23 + this._name.GetHashCode();
				this._hashCode = this._hashCode * 23 + this.FontFamily.GetHashCode();
				this._hashCode = this._hashCode * 23 + this._size.GetHashCode();
				this._hashCode = this._hashCode * 23 + this._unit.GetHashCode();
				this._hashCode = this._hashCode * 23 + this._style.GetHashCode();
				this._hashCode = this._hashCode * 23 + (int)this._gdiCharSet;
				this._hashCode = this._hashCode * 23 + this._gdiVerticalFont.GetHashCode();
			}
			return this._hashCode;
		}

		[MonoTODO("The hdc parameter has no direct equivalent in libgdiplus.")]
		public static Font FromHdc(IntPtr hdc)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("The returned font may not have all it's properties initialized correctly.")]
		public static Font FromLogFont(object lf, IntPtr hdc)
		{
			LOGFONT lOGFONT = (LOGFONT)lf;
			IntPtr newFontObject;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateFontFromLogfont(hdc, ref lOGFONT, out newFontObject));
			return new Font(newFontObject, "Microsoft Sans Serif", FontStyle.Regular, 10f);
		}

		public float GetHeight()
		{
			return this.GetHeight(Graphics.systemDpiY);
		}

		public static Font FromLogFont(object lf)
		{
			if (GDIPlus.RunningOnUnix())
			{
				return Font.FromLogFont(lf, IntPtr.Zero);
			}
			IntPtr intPtr = IntPtr.Zero;
			Font result;
			try
			{
				intPtr = GDIPlus.GetDC(IntPtr.Zero);
				result = Font.FromLogFont(lf, intPtr);
			}
			finally
			{
				GDIPlus.ReleaseDC(IntPtr.Zero, intPtr);
			}
			return result;
		}

		public void ToLogFont(object logFont)
		{
			if (GDIPlus.RunningOnUnix())
			{
				using (Bitmap bitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb))
				{
					using (Graphics graphics = Graphics.FromImage(bitmap))
					{
						this.ToLogFont(logFont, graphics);
						return;
					}
				}
			}
			IntPtr dC = GDIPlus.GetDC(IntPtr.Zero);
			try
			{
				using (Graphics graphics2 = Graphics.FromHdc(dC))
				{
					this.ToLogFont(logFont, graphics2);
				}
			}
			finally
			{
				GDIPlus.ReleaseDC(IntPtr.Zero, dC);
			}
		}

		public void ToLogFont(object logFont, Graphics graphics)
		{
			if (graphics == null)
			{
				throw new ArgumentNullException("graphics");
			}
			if (logFont == null)
			{
				throw new AccessViolationException("logFont");
			}
			if (!logFont.GetType().IsLayoutSequential)
			{
				throw new ArgumentException("logFont", Locale.GetText("Layout must be sequential."));
			}
			Type typeFromHandle = typeof(LOGFONT);
			int num = Marshal.SizeOf(logFont);
			if (num >= Marshal.SizeOf(typeFromHandle))
			{
				IntPtr intPtr = Marshal.AllocHGlobal(num);
				Status status;
				try
				{
					Marshal.StructureToPtr(logFont, intPtr, false);
					status = GDIPlus.GdipGetLogFont(this.NativeObject, graphics.NativeObject, logFont);
					if (status != Status.Ok)
					{
						Marshal.PtrToStructure(intPtr, logFont);
					}
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
				if (Font.CharSetOffset == -1)
				{
					Font.CharSetOffset = (int)Marshal.OffsetOf(typeFromHandle, "lfCharSet");
				}
				GCHandle gCHandle = GCHandle.Alloc(logFont, GCHandleType.Pinned);
				try
				{
					IntPtr intPtr2 = gCHandle.AddrOfPinnedObject();
					if (Marshal.ReadByte(intPtr2, Font.CharSetOffset) == 0)
					{
						Marshal.WriteByte(intPtr2, Font.CharSetOffset, 1);
					}
				}
				finally
				{
					gCHandle.Free();
				}
				GDIPlus.CheckStatus(status);
			}
		}

		public float GetHeight(Graphics graphics)
		{
			if (graphics == null)
			{
				throw new ArgumentNullException("graphics");
			}
			float result;
			GDIPlus.CheckStatus(GDIPlus.GdipGetFontHeight(this.fontObject, graphics.NativeObject, out result));
			return result;
		}

		public float GetHeight(float dpi)
		{
			float result;
			GDIPlus.CheckStatus(GDIPlus.GdipGetFontHeightGivenDPI(this.fontObject, dpi, out result));
			return result;
		}

		public override string ToString()
		{
			return string.Format("[Font: Name={0}, Size={1}, Units={2}, GdiCharSet={3}, GdiVerticalFont={4}]", new object[]
			{
				this._name,
				this.Size,
				(int)this._unit,
				this._gdiCharSet,
				this._gdiVerticalFont
			});
		}
	}
}
