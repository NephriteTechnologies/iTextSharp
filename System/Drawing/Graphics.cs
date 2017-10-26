using System;
using System.ComponentModel;
using iTextSharp.Drawing.Drawing2D;
using iTextSharp.Drawing.Imaging;
using iTextSharp.Drawing.Text;
using System.Runtime.InteropServices;

namespace iTextSharp.Drawing
{
	public sealed class Graphics : MarshalByRefObject, IDisposable, IDeviceContext
	{
		public delegate bool EnumerateMetafileProc(EmfPlusRecordType recordType, int flags, int dataSize, IntPtr data, PlayRecordCallback callbackData);

		public delegate bool DrawImageAbort(IntPtr callbackdata);

		internal IntPtr nativeObject = IntPtr.Zero;

		internal IMacContext maccontext;

		private bool disposed;

		private static float defDpiX;

		private static float defDpiY;

		private IntPtr deviceContextHdc;

		private const string MetafileEnumeration = "Metafiles enumeration, for both WMF and EMF formats, isn't supported.";

		internal static float systemDpiX
		{
			get
			{
				if (Graphics.defDpiX == 0f)
				{
					Graphics expr_18 = Graphics.FromImage(new Bitmap(1, 1));
					Graphics.defDpiX = expr_18.DpiX;
					Graphics.defDpiY = expr_18.DpiY;
				}
				return Graphics.defDpiX;
			}
		}

		internal static float systemDpiY
		{
			get
			{
				if (Graphics.defDpiY == 0f)
				{
					Graphics expr_18 = Graphics.FromImage(new Bitmap(1, 1));
					Graphics.defDpiX = expr_18.DpiX;
					Graphics.defDpiY = expr_18.DpiY;
				}
				return Graphics.defDpiY;
			}
		}

		internal IntPtr NativeObject
		{
			get
			{
				return this.nativeObject;
			}
			set
			{
				this.nativeObject = value;
			}
		}

		public Region Clip
		{
			get
			{
				Region region = new Region();
				GDIPlus.CheckStatus(GDIPlus.GdipGetClip(this.nativeObject, region.NativeObject));
				return region;
			}
			set
			{
				this.SetClip(value, CombineMode.Replace);
			}
		}

		public RectangleF ClipBounds
		{
			get
			{
				RectangleF result = default(RectangleF);
				GDIPlus.CheckStatus(GDIPlus.GdipGetClipBounds(this.nativeObject, out result));
				return result;
			}
		}

		public CompositingMode CompositingMode
		{
			get
			{
				CompositingMode result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetCompositingMode(this.nativeObject, out result));
				return result;
			}
			set
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetCompositingMode(this.nativeObject, value));
			}
		}

		public CompositingQuality CompositingQuality
		{
			get
			{
				CompositingQuality result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetCompositingQuality(this.nativeObject, out result));
				return result;
			}
			set
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetCompositingQuality(this.nativeObject, value));
			}
		}

		public float DpiX
		{
			get
			{
				float result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetDpiX(this.nativeObject, out result));
				return result;
			}
		}

		public float DpiY
		{
			get
			{
				float result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetDpiY(this.nativeObject, out result));
				return result;
			}
		}

		public InterpolationMode InterpolationMode
		{
			get
			{
				InterpolationMode result = InterpolationMode.Invalid;
				GDIPlus.CheckStatus(GDIPlus.GdipGetInterpolationMode(this.nativeObject, out result));
				return result;
			}
			set
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetInterpolationMode(this.nativeObject, value));
			}
		}

		public bool IsClipEmpty
		{
			get
			{
				bool result = false;
				GDIPlus.CheckStatus(GDIPlus.GdipIsClipEmpty(this.nativeObject, out result));
				return result;
			}
		}

		public bool IsVisibleClipEmpty
		{
			get
			{
				bool result = false;
				GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleClipEmpty(this.nativeObject, out result));
				return result;
			}
		}

		public float PageScale
		{
			get
			{
				float result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPageScale(this.nativeObject, out result));
				return result;
			}
			set
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetPageScale(this.nativeObject, value));
			}
		}

		public GraphicsUnit PageUnit
		{
			get
			{
				GraphicsUnit result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPageUnit(this.nativeObject, out result));
				return result;
			}
			set
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetPageUnit(this.nativeObject, value));
			}
		}

		[MonoTODO("This property does not do anything when used with libgdiplus.")]
		public PixelOffsetMode PixelOffsetMode
		{
			get
			{
				PixelOffsetMode result = PixelOffsetMode.Invalid;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPixelOffsetMode(this.nativeObject, out result));
				return result;
			}
			set
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetPixelOffsetMode(this.nativeObject, value));
			}
		}

		public Point RenderingOrigin
		{
			get
			{
				int num;
				int num2;
				GDIPlus.CheckStatus(GDIPlus.GdipGetRenderingOrigin(this.nativeObject, out num, out num2));
				return new Point(num, num2);
			}
			set
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetRenderingOrigin(this.nativeObject, value.X, value.Y));
			}
		}

		public SmoothingMode SmoothingMode
		{
			get
			{
				SmoothingMode result = SmoothingMode.Invalid;
				GDIPlus.CheckStatus(GDIPlus.GdipGetSmoothingMode(this.nativeObject, out result));
				return result;
			}
			set
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetSmoothingMode(this.nativeObject, value));
			}
		}

		[MonoTODO("This property does not do anything when used with libgdiplus.")]
		public int TextContrast
		{
			get
			{
				int result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetTextContrast(this.nativeObject, out result));
				return result;
			}
			set
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetTextContrast(this.nativeObject, value));
			}
		}

		public TextRenderingHint TextRenderingHint
		{
			get
			{
				TextRenderingHint result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetTextRenderingHint(this.nativeObject, out result));
				return result;
			}
			set
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetTextRenderingHint(this.nativeObject, value));
			}
		}

		public Matrix Transform
		{
			get
			{
				Matrix matrix = new Matrix();
				GDIPlus.CheckStatus(GDIPlus.GdipGetWorldTransform(this.nativeObject, matrix.nativeMatrix));
				return matrix;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				GDIPlus.CheckStatus(GDIPlus.GdipSetWorldTransform(this.nativeObject, value.nativeMatrix));
			}
		}

		public RectangleF VisibleClipBounds
		{
			get
			{
				RectangleF result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetVisibleClipBounds(this.nativeObject, out result));
				return result;
			}
		}

		internal Graphics(IntPtr nativeGraphics)
		{
			this.nativeObject = nativeGraphics;
		}

		~Graphics()
		{
			this.Dispose();
		}

		[MonoTODO("Metafiles, both WMF and EMF formats, aren't supported.")]
		public void AddMetafileComment(byte[] data)
		{
			throw new NotImplementedException();
		}

		public GraphicsContainer BeginContainer()
		{
			uint state;
			GDIPlus.CheckStatus(GDIPlus.GdipBeginContainer2(this.nativeObject, out state));
			return new GraphicsContainer(state);
		}

		[MonoTODO("The rectangles and unit parameters aren't supported in libgdiplus")]
		public GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit)
		{
			uint state;
			GDIPlus.CheckStatus(GDIPlus.GdipBeginContainerI(this.nativeObject, ref dstrect, ref srcrect, unit, out state));
			return new GraphicsContainer(state);
		}

		[MonoTODO("The rectangles and unit parameters aren't supported in libgdiplus")]
		public GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit)
		{
			uint state;
			GDIPlus.CheckStatus(GDIPlus.GdipBeginContainer(this.nativeObject, ref dstrect, ref srcrect, unit, out state));
			return new GraphicsContainer(state);
		}

		public void Clear(Color color)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGraphicsClear(this.nativeObject, color.ToArgb()));
		}

		[MonoLimitation("Works on Win32 and on X11 (but not on Cocoa and Quartz)")]
		public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize)
		{
			this.CopyFromScreen(upperLeftSource.X, upperLeftSource.Y, upperLeftDestination.X, upperLeftDestination.Y, blockRegionSize, CopyPixelOperation.SourceCopy);
		}

		[MonoLimitation("Works on Win32 and (for CopyPixelOperation.SourceCopy only) on X11 but not on Cocoa and Quartz")]
		public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
		{
			this.CopyFromScreen(upperLeftSource.X, upperLeftSource.Y, upperLeftDestination.X, upperLeftDestination.Y, blockRegionSize, copyPixelOperation);
		}

		[MonoLimitation("Works on Win32 and on X11 (but not on Cocoa and Quartz)")]
		public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize)
		{
			this.CopyFromScreen(sourceX, sourceY, destinationX, destinationY, blockRegionSize, CopyPixelOperation.SourceCopy);
		}

		[MonoLimitation("Works on Win32 and (for CopyPixelOperation.SourceCopy only) on X11 but not on Cocoa and Quartz")]
		public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
		{
			if (!Enum.IsDefined(typeof(CopyPixelOperation), copyPixelOperation))
			{
				throw new InvalidEnumArgumentException(Locale.GetText("Enum argument value '{0}' is not valid for CopyPixelOperation", new object[]
				{
					copyPixelOperation
				}));
			}
			if (GDIPlus.UseX11Drawable)
			{
				this.CopyFromScreenX11(sourceX, sourceY, destinationX, destinationY, blockRegionSize, copyPixelOperation);
				return;
			}
			if (GDIPlus.UseCarbonDrawable)
			{
				this.CopyFromScreenMac(sourceX, sourceY, destinationX, destinationY, blockRegionSize, copyPixelOperation);
				return;
			}
			if (GDIPlus.UseCocoaDrawable)
			{
				this.CopyFromScreenMac(sourceX, sourceY, destinationX, destinationY, blockRegionSize, copyPixelOperation);
				return;
			}
			this.CopyFromScreenWin32(sourceX, sourceY, destinationX, destinationY, blockRegionSize, copyPixelOperation);
		}

		private void CopyFromScreenWin32(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
		{
			IntPtr dC = GDIPlus.GetDC(GDIPlus.GetDesktopWindow());
			IntPtr hdc = this.GetHdc();
			GDIPlus.BitBlt(hdc, destinationX, destinationY, blockRegionSize.Width, blockRegionSize.Height, dC, sourceX, sourceY, (int)copyPixelOperation);
			GDIPlus.ReleaseDC(IntPtr.Zero, dC);
			this.ReleaseHdc(hdc);
		}

		private void CopyFromScreenMac(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
		{
			throw new NotImplementedException();
		}

		private void CopyFromScreenX11(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
		{
			int pane = -1;
			int num = 0;
			if (copyPixelOperation != CopyPixelOperation.SourceCopy)
			{
				throw new NotImplementedException("Operation not implemented under X11");
			}
			if (GDIPlus.Display == IntPtr.Zero)
			{
				GDIPlus.Display = GDIPlus.XOpenDisplay(IntPtr.Zero);
			}
			IntPtr drawable = GDIPlus.XRootWindow(GDIPlus.Display, 0);
			IntPtr visual = GDIPlus.XDefaultVisual(GDIPlus.Display, 0);
			XVisualInfo xVisualInfo = default(XVisualInfo);
			xVisualInfo.visualid = GDIPlus.XVisualIDFromVisual(visual);
			IntPtr intPtr = GDIPlus.XGetVisualInfo(GDIPlus.Display, 1, ref xVisualInfo, ref num);
			xVisualInfo = (XVisualInfo)Marshal.PtrToStructure(intPtr, typeof(XVisualInfo));
			IntPtr intPtr2 = GDIPlus.XGetImage(GDIPlus.Display, drawable, sourceX, sourceY, blockRegionSize.Width, blockRegionSize.Height, pane, 2);
			if (intPtr2 == IntPtr.Zero)
			{
				throw new InvalidOperationException(string.Format("XGetImage returned NULL when asked to for a {0}x{1} region block", blockRegionSize.Width, blockRegionSize.Height));
			}
			Bitmap bitmap = new Bitmap(blockRegionSize.Width, blockRegionSize.Height);
			int num2 = (int)xVisualInfo.red_mask;
			int num3 = (int)xVisualInfo.blue_mask;
			int num4 = (int)xVisualInfo.green_mask;
			for (int i = 0; i < blockRegionSize.Height; i++)
			{
				for (int j = 0; j < blockRegionSize.Width; j++)
				{
					int num5 = GDIPlus.XGetPixel(intPtr2, j, i);
					uint depth = xVisualInfo.depth;
					int num6;
					int num7;
					int num8;
					if (depth != 16u)
					{
						if (depth != 24u && depth != 32u)
						{
							throw new NotImplementedException(Locale.GetText("{0}bbp depth not supported.", new object[]
							{
								xVisualInfo.depth
							}));
						}
						num6 = ((num5 & num2) >> 16 & 255);
						num7 = ((num5 & num4) >> 8 & 255);
						num8 = (num5 & num3 & 255);
					}
					else
					{
						num6 = ((num5 & num2) >> 8 & 255);
						num7 = ((num5 & num4) >> 3 & 255);
						num8 = ((num5 & num3) << 3 & 255);
					}
					bitmap.SetPixel(j, i, Color.FromArgb(255, num6, num7, num8));
				}
			}
			this.DrawImage(bitmap, destinationX, destinationY);
			bitmap.Dispose();
			GDIPlus.XDestroyImage(intPtr2);
			GDIPlus.XFree(intPtr);
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				if (GDIPlus.UseCarbonDrawable || GDIPlus.UseCocoaDrawable)
				{
					this.Flush();
					if (this.maccontext != null)
					{
						this.maccontext.Release();
					}
				}
				Status arg_45_0 = GDIPlus.GdipDeleteGraphics(this.nativeObject);
				this.nativeObject = IntPtr.Zero;
				GDIPlus.CheckStatus(arg_45_0);
				this.disposed = true;
			}
			GC.SuppressFinalize(this);
		}

		public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
		{
			this.DrawArc(pen, (float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height, startAngle, sweepAngle);
		}

		public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
		{
			this.DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawArc(this.nativeObject, pen.nativeObject, x, y, width, height, startAngle, sweepAngle));
		}

		public void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawArcI(this.nativeObject, pen.nativeObject, x, y, width, height, (float)startAngle, (float)sweepAngle));
		}

		public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawBezier(this.nativeObject, pen.nativeObject, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y));
		}

		public void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawBezierI(this.nativeObject, pen.nativeObject, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y));
		}

		public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawBezier(this.nativeObject, pen.nativeObject, x1, y1, x2, y2, x3, y3, x4, y4));
		}

		public void DrawBeziers(Pen pen, Point[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			int num = points.Length;
			if (num < 4)
			{
				return;
			}
			for (int i = 0; i < num - 1; i += 3)
			{
				Point point = points[i];
				Point point2 = points[i + 1];
				Point point3 = points[i + 2];
				Point point4 = points[i + 3];
				GDIPlus.CheckStatus(GDIPlus.GdipDrawBezier(this.nativeObject, pen.nativeObject, (float)point.X, (float)point.Y, (float)point2.X, (float)point2.Y, (float)point3.X, (float)point3.Y, (float)point4.X, (float)point4.Y));
			}
		}

		public void DrawBeziers(Pen pen, PointF[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			int num = points.Length;
			if (num < 4)
			{
				return;
			}
			for (int i = 0; i < num - 1; i += 3)
			{
				PointF pointF = points[i];
				PointF pointF2 = points[i + 1];
				PointF pointF3 = points[i + 2];
				PointF pointF4 = points[i + 3];
				GDIPlus.CheckStatus(GDIPlus.GdipDrawBezier(this.nativeObject, pen.nativeObject, pointF.X, pointF.Y, pointF2.X, pointF2.Y, pointF3.X, pointF3.Y, pointF4.X, pointF4.Y));
			}
		}

		public void DrawClosedCurve(Pen pen, PointF[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawClosedCurve(this.nativeObject, pen.nativeObject, points, points.Length));
		}

		public void DrawClosedCurve(Pen pen, Point[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawClosedCurveI(this.nativeObject, pen.nativeObject, points, points.Length));
		}

		public void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawClosedCurve2I(this.nativeObject, pen.nativeObject, points, points.Length, tension));
		}

		public void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawClosedCurve2(this.nativeObject, pen.nativeObject, points, points.Length, tension));
		}

		public void DrawCurve(Pen pen, Point[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawCurveI(this.nativeObject, pen.nativeObject, points, points.Length));
		}

		public void DrawCurve(Pen pen, PointF[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawCurve(this.nativeObject, pen.nativeObject, points, points.Length));
		}

		public void DrawCurve(Pen pen, PointF[] points, float tension)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawCurve2(this.nativeObject, pen.nativeObject, points, points.Length, tension));
		}

		public void DrawCurve(Pen pen, Point[] points, float tension)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawCurve2I(this.nativeObject, pen.nativeObject, points, points.Length, tension));
		}

		public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawCurve3(this.nativeObject, pen.nativeObject, points, points.Length, offset, numberOfSegments, 0.5f));
		}

		public void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawCurve3I(this.nativeObject, pen.nativeObject, points, points.Length, offset, numberOfSegments, tension));
		}

		public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawCurve3(this.nativeObject, pen.nativeObject, points, points.Length, offset, numberOfSegments, tension));
		}

		public void DrawEllipse(Pen pen, Rectangle rect)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			this.DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void DrawEllipse(Pen pen, RectangleF rect)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			this.DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void DrawEllipse(Pen pen, int x, int y, int width, int height)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawEllipseI(this.nativeObject, pen.nativeObject, x, y, width, height));
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawEllipse(this.nativeObject, pen.nativeObject, x, y, width, height));
		}

		public void DrawIcon(Icon icon, Rectangle targetRect)
		{
			if (icon == null)
			{
				throw new ArgumentNullException("icon");
			}
			this.DrawImage(icon.GetInternalBitmap(), targetRect);
		}

		public void DrawIcon(Icon icon, int x, int y)
		{
			if (icon == null)
			{
				throw new ArgumentNullException("icon");
			}
			this.DrawImage(icon.GetInternalBitmap(), x, y);
		}

		public void DrawIconUnstretched(Icon icon, Rectangle targetRect)
		{
			if (icon == null)
			{
				throw new ArgumentNullException("icon");
			}
			this.DrawImageUnscaled(icon.GetInternalBitmap(), targetRect);
		}

		public void DrawImage(Image image, RectangleF rect)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRect(this.nativeObject, image.NativeObject, rect.X, rect.Y, rect.Width, rect.Height));
		}

		public void DrawImage(Image image, PointF point)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImage(this.nativeObject, image.NativeObject, point.X, point.Y));
		}

		public void DrawImage(Image image, Point[] destPoints)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsI(this.nativeObject, image.NativeObject, destPoints, destPoints.Length));
		}

		public void DrawImage(Image image, Point point)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			this.DrawImage(image, point.X, point.Y);
		}

		public void DrawImage(Image image, Rectangle rect)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			this.DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void DrawImage(Image image, PointF[] destPoints)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePoints(this.nativeObject, image.NativeObject, destPoints, destPoints.Length));
		}

		public void DrawImage(Image image, int x, int y)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImageI(this.nativeObject, image.NativeObject, x, y));
		}

		public void DrawImage(Image image, float x, float y)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImage(this.nativeObject, image.NativeObject, x, y));
		}

		public void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRectI(this.nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero, null, IntPtr.Zero));
		}

		public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRect(this.nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero, null, IntPtr.Zero));
		}

		public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsRectI(this.nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero, null, IntPtr.Zero));
		}

		public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsRect(this.nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero, null, IntPtr.Zero));
		}

		public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsRectI(this.nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, (imageAttr != null) ? imageAttr.NativeObject : IntPtr.Zero, null, IntPtr.Zero));
		}

		public void DrawImage(Image image, float x, float y, float width, float height)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRect(this.nativeObject, image.NativeObject, x, y, width, height));
		}

		public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsRect(this.nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, (imageAttr != null) ? imageAttr.NativeObject : IntPtr.Zero, null, IntPtr.Zero));
		}

		public void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointRectI(this.nativeObject, image.NativeObject, x, y, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit));
		}

		public void DrawImage(Image image, int x, int y, int width, int height)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectI(this.nativeObject, image.nativeObject, x, y, width, height));
		}

		public void DrawImage(Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointRect(this.nativeObject, image.nativeObject, x, y, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit));
		}

		public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsRect(this.nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, (imageAttr != null) ? imageAttr.NativeObject : IntPtr.Zero, callback, IntPtr.Zero));
		}

		public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsRectI(this.nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, (imageAttr != null) ? imageAttr.NativeObject : IntPtr.Zero, callback, IntPtr.Zero));
		}

		public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback, int callbackData)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (destPoints == null)
			{
				throw new ArgumentNullException("destPoints");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsRectI(this.nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, (imageAttr != null) ? imageAttr.NativeObject : IntPtr.Zero, callback, (IntPtr)callbackData));
		}

		public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRect(this.nativeObject, image.NativeObject, (float)destRect.X, (float)destRect.Y, (float)destRect.Width, (float)destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, IntPtr.Zero, null, IntPtr.Zero));
		}

		public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback, int callbackData)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsRect(this.nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, (imageAttr != null) ? imageAttr.NativeObject : IntPtr.Zero, callback, (IntPtr)callbackData));
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRectI(this.nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, IntPtr.Zero, null, IntPtr.Zero));
		}

		public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRect(this.nativeObject, image.NativeObject, (float)destRect.X, (float)destRect.Y, (float)destRect.Width, (float)destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, (imageAttrs != null) ? imageAttrs.NativeObject : IntPtr.Zero, null, IntPtr.Zero));
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRectI(this.nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, (imageAttr != null) ? imageAttr.NativeObject : IntPtr.Zero, null, IntPtr.Zero));
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRectI(this.nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, (imageAttr != null) ? imageAttr.NativeObject : IntPtr.Zero, callback, IntPtr.Zero));
		}

		public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, Graphics.DrawImageAbort callback)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRect(this.nativeObject, image.NativeObject, (float)destRect.X, (float)destRect.Y, (float)destRect.Width, (float)destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, (imageAttrs != null) ? imageAttrs.NativeObject : IntPtr.Zero, callback, IntPtr.Zero));
		}

		public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, Graphics.DrawImageAbort callback, IntPtr callbackData)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRect(this.nativeObject, image.NativeObject, (float)destRect.X, (float)destRect.Y, (float)destRect.Width, (float)destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, (imageAttrs != null) ? imageAttrs.NativeObject : IntPtr.Zero, callback, callbackData));
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, Graphics.DrawImageAbort callback, IntPtr callbackData)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRect(this.nativeObject, image.NativeObject, (float)destRect.X, (float)destRect.Y, (float)destRect.Width, (float)destRect.Height, (float)srcX, (float)srcY, (float)srcWidth, (float)srcHeight, srcUnit, (imageAttrs != null) ? imageAttrs.NativeObject : IntPtr.Zero, callback, callbackData));
		}

		public void DrawImageUnscaled(Image image, Point point)
		{
			this.DrawImageUnscaled(image, point.X, point.Y);
		}

		public void DrawImageUnscaled(Image image, Rectangle rect)
		{
			this.DrawImageUnscaled(image, rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void DrawImageUnscaled(Image image, int x, int y)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			this.DrawImage(image, x, y, image.Width, image.Height);
		}

		public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (width <= 0 || height <= 0)
			{
				return;
			}
			using (Image image2 = new Bitmap(width, height))
			{
				using (Graphics graphics = Graphics.FromImage(image2))
				{
					graphics.DrawImage(image, 0, 0, image.Width, image.Height);
					this.DrawImage(image2, x, y, width, height);
				}
			}
		}

		public void DrawImageUnscaledAndClipped(Image image, Rectangle rect)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			int width = (image.Width > rect.Width) ? rect.Width : image.Width;
			int height = (image.Height > rect.Height) ? rect.Height : image.Height;
			this.DrawImageUnscaled(image, rect.X, rect.Y, width, height);
		}

		public void DrawLine(Pen pen, PointF pt1, PointF pt2)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawLine(this.nativeObject, pen.nativeObject, pt1.X, pt1.Y, pt2.X, pt2.Y));
		}

		public void DrawLine(Pen pen, Point pt1, Point pt2)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawLineI(this.nativeObject, pen.nativeObject, pt1.X, pt1.Y, pt2.X, pt2.Y));
		}

		public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawLineI(this.nativeObject, pen.nativeObject, x1, y1, x2, y2));
		}

		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (!float.IsNaN(x1) && !float.IsNaN(y1) && !float.IsNaN(x2) && !float.IsNaN(y2))
			{
				GDIPlus.CheckStatus(GDIPlus.GdipDrawLine(this.nativeObject, pen.nativeObject, x1, y1, x2, y2));
			}
		}

		public void DrawLines(Pen pen, PointF[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawLines(this.nativeObject, pen.nativeObject, points, points.Length));
		}

		public void DrawLines(Pen pen, Point[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawLinesI(this.nativeObject, pen.nativeObject, points, points.Length));
		}

		public void DrawPath(Pen pen, GraphicsPath path)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawPath(this.nativeObject, pen.nativeObject, path.nativePath));
		}

		public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			this.DrawPie(pen, (float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height, startAngle, sweepAngle);
		}

		public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			this.DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
		}

		public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawPie(this.nativeObject, pen.nativeObject, x, y, width, height, startAngle, sweepAngle));
		}

		public void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawPieI(this.nativeObject, pen.nativeObject, x, y, width, height, (float)startAngle, (float)sweepAngle));
		}

		public void DrawPolygon(Pen pen, Point[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawPolygonI(this.nativeObject, pen.nativeObject, points, points.Length));
		}

		public void DrawPolygon(Pen pen, PointF[] points)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawPolygon(this.nativeObject, pen.nativeObject, points, points.Length));
		}

		public void DrawRectangle(Pen pen, Rectangle rect)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			this.DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawRectangle(this.nativeObject, pen.nativeObject, x, y, width, height));
		}

		public void DrawRectangle(Pen pen, int x, int y, int width, int height)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("pen");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawRectangleI(this.nativeObject, pen.nativeObject, x, y, width, height));
		}

		public void DrawRectangles(Pen pen, RectangleF[] rects)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("image");
			}
			if (rects == null)
			{
				throw new ArgumentNullException("rects");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawRectangles(this.nativeObject, pen.nativeObject, rects, rects.Length));
		}

		public void DrawRectangles(Pen pen, Rectangle[] rects)
		{
			if (pen == null)
			{
				throw new ArgumentNullException("image");
			}
			if (rects == null)
			{
				throw new ArgumentNullException("rects");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawRectanglesI(this.nativeObject, pen.nativeObject, rects, rects.Length));
		}

		public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle)
		{
			this.DrawString(s, font, brush, layoutRectangle, null);
		}

		public void DrawString(string s, Font font, Brush brush, PointF point)
		{
			this.DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0f, 0f), null);
		}

		public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
		{
			this.DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0f, 0f), format);
		}

		public void DrawString(string s, Font font, Brush brush, float x, float y)
		{
			this.DrawString(s, font, brush, new RectangleF(x, y, 0f, 0f), null);
		}

		public void DrawString(string s, Font font, Brush brush, float x, float y, StringFormat format)
		{
			this.DrawString(s, font, brush, new RectangleF(x, y, 0f, 0f), format);
		}

		public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
		{
			if (font == null)
			{
				throw new ArgumentNullException("font");
			}
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (s == null || s.Length == 0)
			{
				return;
			}
			GDIPlus.CheckStatus(GDIPlus.GdipDrawString(this.nativeObject, s, s.Length, font.NativeObject, ref layoutRectangle, (format != null) ? format.NativeObject : IntPtr.Zero, brush.nativeObject));
		}

		public void EndContainer(GraphicsContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipEndContainer(this.nativeObject, container.NativeObject));
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point destPoint, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF destPoint, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF destPoint, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point destPoint, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point destPoint, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF destPoint, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
		public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
		{
			throw new NotImplementedException();
		}

		public void ExcludeClip(Rectangle rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetClipRectI(this.nativeObject, rect.X, rect.Y, rect.Width, rect.Height, CombineMode.Exclude));
		}

		public void ExcludeClip(Region region)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipSetClipRegion(this.nativeObject, region.NativeObject, CombineMode.Exclude));
		}

		public void FillClosedCurve(Brush brush, PointF[] points)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillClosedCurve(this.nativeObject, brush.NativeObject, points, points.Length));
		}

		public void FillClosedCurve(Brush brush, Point[] points)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillClosedCurveI(this.nativeObject, brush.NativeObject, points, points.Length));
		}

		public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			this.FillClosedCurve(brush, points, fillmode, 0.5f);
		}

		public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			this.FillClosedCurve(brush, points, fillmode, 0.5f);
		}

		public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillClosedCurve2(this.nativeObject, brush.NativeObject, points, points.Length, tension, fillmode));
		}

		public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillClosedCurve2I(this.nativeObject, brush.NativeObject, points, points.Length, tension, fillmode));
		}

		public void FillEllipse(Brush brush, Rectangle rect)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			this.FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void FillEllipse(Brush brush, RectangleF rect)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			this.FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void FillEllipse(Brush brush, float x, float y, float width, float height)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillEllipse(this.nativeObject, brush.nativeObject, x, y, width, height));
		}

		public void FillEllipse(Brush brush, int x, int y, int width, int height)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillEllipseI(this.nativeObject, brush.nativeObject, x, y, width, height));
		}

		public void FillPath(Brush brush, GraphicsPath path)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillPath(this.nativeObject, brush.NativeObject, path.NativeObject));
		}

		public void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillPie(this.nativeObject, brush.NativeObject, (float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height, startAngle, sweepAngle));
		}

		public void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillPieI(this.nativeObject, brush.NativeObject, x, y, width, height, (float)startAngle, (float)sweepAngle));
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillPie(this.nativeObject, brush.NativeObject, x, y, width, height, startAngle, sweepAngle));
		}

		public void FillPolygon(Brush brush, PointF[] points)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillPolygon2(this.nativeObject, brush.nativeObject, points, points.Length));
		}

		public void FillPolygon(Brush brush, Point[] points)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillPolygon2I(this.nativeObject, brush.nativeObject, points, points.Length));
		}

		public void FillPolygon(Brush brush, Point[] points, FillMode fillMode)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillPolygonI(this.nativeObject, brush.nativeObject, points, points.Length, fillMode));
		}

		public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillPolygon(this.nativeObject, brush.nativeObject, points, points.Length, fillMode));
		}

		public void FillRectangle(Brush brush, RectangleF rect)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			this.FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);
		}

		public void FillRectangle(Brush brush, Rectangle rect)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			this.FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);
		}

		public void FillRectangle(Brush brush, int x, int y, int width, int height)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillRectangleI(this.nativeObject, brush.nativeObject, x, y, width, height));
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillRectangle(this.nativeObject, brush.nativeObject, x, y, width, height));
		}

		public void FillRectangles(Brush brush, Rectangle[] rects)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (rects == null)
			{
				throw new ArgumentNullException("rects");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillRectanglesI(this.nativeObject, brush.nativeObject, rects, rects.Length));
		}

		public void FillRectangles(Brush brush, RectangleF[] rects)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (rects == null)
			{
				throw new ArgumentNullException("rects");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillRectangles(this.nativeObject, brush.nativeObject, rects, rects.Length));
		}

		public void FillRegion(Brush brush, Region region)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFillRegion(this.nativeObject, brush.NativeObject, region.NativeObject));
		}

		public void Flush()
		{
			this.Flush(FlushIntention.Flush);
		}

		public void Flush(FlushIntention intention)
		{
			if (this.nativeObject == IntPtr.Zero)
			{
				return;
			}
			GDIPlus.CheckStatus(GDIPlus.GdipFlush(this.nativeObject, intention));
			if (this.maccontext != null)
			{
				this.maccontext.Synchronize();
			}
		}

		[EditorBrowsable]
		public static Graphics FromHdc(IntPtr hdc)
		{
			IntPtr nativeGraphics;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateFromHDC(hdc, out nativeGraphics));
			return new Graphics(nativeGraphics);
		}

		[EditorBrowsable, MonoTODO]
		public static Graphics FromHdc(IntPtr hdc, IntPtr hdevice)
		{
			throw new NotImplementedException();
		}

		[EditorBrowsable]
		public static Graphics FromHdcInternal(IntPtr hdc)
		{
			GDIPlus.Display = hdc;
			return null;
		}

		[EditorBrowsable]
		public static Graphics FromHwnd(IntPtr hwnd)
		{
			IntPtr nativeGraphics;
			if (GDIPlus.UseCocoaDrawable)
			{
				CocoaContext cGContextForNSView = MacSupport.GetCGContextForNSView(hwnd);
				GDIPlus.GdipCreateFromContext_macosx(cGContextForNSView.ctx, cGContextForNSView.width, cGContextForNSView.height, out nativeGraphics);
				return new Graphics(nativeGraphics)
				{
					maccontext = cGContextForNSView
				};
			}
			if (GDIPlus.UseCarbonDrawable)
			{
				CarbonContext cGContextForView = MacSupport.GetCGContextForView(hwnd);
				GDIPlus.GdipCreateFromContext_macosx(cGContextForView.ctx, cGContextForView.width, cGContextForView.height, out nativeGraphics);
				return new Graphics(nativeGraphics)
				{
					maccontext = cGContextForView
				};
			}
			if (GDIPlus.UseX11Drawable)
			{
				if (GDIPlus.Display == IntPtr.Zero)
				{
					GDIPlus.Display = GDIPlus.XOpenDisplay(IntPtr.Zero);
					if (GDIPlus.Display == IntPtr.Zero)
					{
						throw new NotSupportedException("Could not open display (X-Server required. Check your DISPLAY environment variable)");
					}
				}
				if (hwnd == IntPtr.Zero)
				{
					hwnd = GDIPlus.XRootWindow(GDIPlus.Display, GDIPlus.XDefaultScreen(GDIPlus.Display));
				}
				return Graphics.FromXDrawable(hwnd, GDIPlus.Display);
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCreateFromHWND(hwnd, out nativeGraphics));
			return new Graphics(nativeGraphics);
		}

		[EditorBrowsable]
		public static Graphics FromHwndInternal(IntPtr hwnd)
		{
			return Graphics.FromHwnd(hwnd);
		}

		public static Graphics FromImage(Image image)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if ((image.PixelFormat & PixelFormat.Indexed) != PixelFormat.DontCare)
			{
				throw new Exception(Locale.GetText("Cannot create Graphics from an indexed bitmap."));
			}
			IntPtr nativeGraphics;
			GDIPlus.CheckStatus(GDIPlus.GdipGetImageGraphicsContext(image.nativeObject, out nativeGraphics));
			Graphics graphics = new Graphics(nativeGraphics);
			if (GDIPlus.RunningOnUnix())
			{
				Rectangle rectangle = new Rectangle(0, 0, image.Width, image.Height);
				GDIPlus.GdipSetVisibleClip_linux(graphics.NativeObject, ref rectangle);
			}
			return graphics;
		}

		internal static Graphics FromXDrawable(IntPtr drawable, IntPtr display)
		{
			IntPtr nativeGraphics;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateFromXDrawable_linux(drawable, display, out nativeGraphics));
			return new Graphics(nativeGraphics);
		}

		[MonoTODO]
		public static IntPtr GetHalftonePalette()
		{
			throw new NotImplementedException();
		}

		public IntPtr GetHdc()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetDC(this.nativeObject, out this.deviceContextHdc));
			return this.deviceContextHdc;
		}

		public Color GetNearestColor(Color color)
		{
			int num;
			GDIPlus.CheckStatus(GDIPlus.GdipGetNearestColor(this.nativeObject, out num));
			return Color.FromArgb(num);
		}

		public void IntersectClip(Region region)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipSetClipRegion(this.nativeObject, region.NativeObject, CombineMode.Intersect));
		}

		public void IntersectClip(RectangleF rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetClipRect(this.nativeObject, rect.X, rect.Y, rect.Width, rect.Height, CombineMode.Intersect));
		}

		public void IntersectClip(Rectangle rect)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetClipRectI(this.nativeObject, rect.X, rect.Y, rect.Width, rect.Height, CombineMode.Intersect));
		}

		public bool IsVisible(Point point)
		{
			bool result = false;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisiblePointI(this.nativeObject, point.X, point.Y, out result));
			return result;
		}

		public bool IsVisible(RectangleF rect)
		{
			bool result = false;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRect(this.nativeObject, rect.X, rect.Y, rect.Width, rect.Height, out result));
			return result;
		}

		public bool IsVisible(PointF point)
		{
			bool result = false;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisiblePoint(this.nativeObject, point.X, point.Y, out result));
			return result;
		}

		public bool IsVisible(Rectangle rect)
		{
			bool result = false;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRectI(this.nativeObject, rect.X, rect.Y, rect.Width, rect.Height, out result));
			return result;
		}

		public bool IsVisible(float x, float y)
		{
			return this.IsVisible(new PointF(x, y));
		}

		public bool IsVisible(int x, int y)
		{
			return this.IsVisible(new Point(x, y));
		}

		public bool IsVisible(float x, float y, float width, float height)
		{
			return this.IsVisible(new RectangleF(x, y, width, height));
		}

		public bool IsVisible(int x, int y, int width, int height)
		{
			return this.IsVisible(new Rectangle(x, y, width, height));
		}

		public Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect, StringFormat stringFormat)
		{
			if (text == null || text.Length == 0)
			{
				return new Region[0];
			}
			if (font == null)
			{
				throw new ArgumentNullException("font");
			}
			if (stringFormat == null)
			{
				throw new ArgumentException("stringFormat");
			}
			int measurableCharacterRangeCount = stringFormat.GetMeasurableCharacterRangeCount();
			if (measurableCharacterRangeCount == 0)
			{
				return new Region[0];
			}
			IntPtr[] array = new IntPtr[measurableCharacterRangeCount];
			Region[] array2 = new Region[measurableCharacterRangeCount];
			for (int i = 0; i < measurableCharacterRangeCount; i++)
			{
				array2[i] = new Region();
				array[i] = array2[i].NativeObject;
			}
			GDIPlus.CheckStatus(GDIPlus.GdipMeasureCharacterRanges(this.nativeObject, text, text.Length, font.NativeObject, ref layoutRect, stringFormat.NativeObject, measurableCharacterRangeCount, out array[0]));
			return array2;
		}

		private unsafe SizeF GdipMeasureString(IntPtr graphics, string text, Font font, ref RectangleF layoutRect, IntPtr stringFormat)
		{
			if (text == null || text.Length == 0)
			{
				return SizeF.Empty;
			}
			if (font == null)
			{
				throw new ArgumentNullException("font");
			}
			RectangleF rectangleF = default(RectangleF);
            Status status = GDIPlus.GdipMeasureString(this.nativeObject, text, text.Length, font.NativeObject, ref layoutRect, stringFormat, out rectangleF, null, null);
            GDIPlus.CheckStatus(status);
			return new SizeF(rectangleF.Width, rectangleF.Height);
		}

		public SizeF MeasureString(string text, Font font)
		{
			return this.MeasureString(text, font, SizeF.Empty);
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea)
		{
			RectangleF rectangleF = new RectangleF(0f, 0f, layoutArea.Width, layoutArea.Height);
			return this.GdipMeasureString(this.nativeObject, text, font, ref rectangleF, IntPtr.Zero);
		}

		public SizeF MeasureString(string text, Font font, int width)
		{
			RectangleF rectangleF = new RectangleF(0f, 0f, (float)width, 2.14748365E+09f);
			return this.GdipMeasureString(this.nativeObject, text, font, ref rectangleF, IntPtr.Zero);
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
		{
			RectangleF rectangleF = new RectangleF(0f, 0f, layoutArea.Width, layoutArea.Height);
			IntPtr stringFormat2 = (stringFormat == null) ? IntPtr.Zero : stringFormat.NativeObject;
			return this.GdipMeasureString(this.nativeObject, text, font, ref rectangleF, stringFormat2);
		}

		public SizeF MeasureString(string text, Font font, int width, StringFormat format)
		{
			RectangleF rectangleF = new RectangleF(0f, 0f, (float)width, 2.14748365E+09f);
			IntPtr stringFormat = (format == null) ? IntPtr.Zero : format.NativeObject;
			return this.GdipMeasureString(this.nativeObject, text, font, ref rectangleF, stringFormat);
		}

		public SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat)
		{
			RectangleF rectangleF = new RectangleF(origin.X, origin.Y, 0f, 0f);
			IntPtr stringFormat2 = (stringFormat == null) ? IntPtr.Zero : stringFormat.NativeObject;
			return this.GdipMeasureString(this.nativeObject, text, font, ref rectangleF, stringFormat2);
		}

		public unsafe SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled)
		{
			charactersFitted = 0;
			linesFilled = 0;
			if (text == null || text.Length == 0)
			{
				return SizeF.Empty;
			}
			if (font == null)
			{
				throw new ArgumentNullException("font");
			}
			RectangleF rectangleF = default(RectangleF);
			RectangleF rectangleF2 = new RectangleF(0f, 0f, layoutArea.Width, layoutArea.Height);
			IntPtr stringFormat2 = (stringFormat == null) ? IntPtr.Zero : stringFormat.NativeObject;
			fixed (int* ptr = &charactersFitted, ptr2 = &linesFilled)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipMeasureString(this.nativeObject, text, text.Length, font.NativeObject, ref rectangleF2, stringFormat2, out rectangleF, ptr, ptr2));
			}
			return new SizeF(rectangleF.Width, rectangleF.Height);
		}

		public void MultiplyTransform(Matrix matrix)
		{
			this.MultiplyTransform(matrix, MatrixOrder.Prepend);
		}

		public void MultiplyTransform(Matrix matrix, MatrixOrder order)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException("matrix");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipMultiplyWorldTransform(this.nativeObject, matrix.nativeMatrix, order));
		}

		[EditorBrowsable]
		public void ReleaseHdc(IntPtr hdc)
		{
			this.ReleaseHdcInternal(hdc);
		}

		public void ReleaseHdc()
		{
			this.ReleaseHdcInternal(this.deviceContextHdc);
		}

		[EditorBrowsable, MonoLimitation("Can only be used when hdc was provided by Graphics.GetHdc() method")]
		public void ReleaseHdcInternal(IntPtr hdc)
		{
			Status status = Status.InvalidParameter;
			if (hdc == this.deviceContextHdc)
			{
				status = GDIPlus.GdipReleaseDC(this.nativeObject, this.deviceContextHdc);
				this.deviceContextHdc = IntPtr.Zero;
			}
			GDIPlus.CheckStatus(status);
		}

		public void ResetClip()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipResetClip(this.nativeObject));
		}

		public void ResetTransform()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipResetWorldTransform(this.nativeObject));
		}

		public void Restore(GraphicsState gstate)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipRestoreGraphics(this.nativeObject, gstate.nativeState));
		}

		public void RotateTransform(float angle)
		{
			this.RotateTransform(angle, MatrixOrder.Prepend);
		}

		public void RotateTransform(float angle, MatrixOrder order)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipRotateWorldTransform(this.nativeObject, angle, order));
		}

		public GraphicsState Save()
		{
			uint nativeState;
			GDIPlus.CheckStatus(GDIPlus.GdipSaveGraphics(this.nativeObject, out nativeState));
			return new GraphicsState
			{
				nativeState = nativeState
			};
		}

		public void ScaleTransform(float sx, float sy)
		{
			this.ScaleTransform(sx, sy, MatrixOrder.Prepend);
		}

		public void ScaleTransform(float sx, float sy, MatrixOrder order)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipScaleWorldTransform(this.nativeObject, sx, sy, order));
		}

		public void SetClip(RectangleF rect)
		{
			this.SetClip(rect, CombineMode.Replace);
		}

		public void SetClip(GraphicsPath path)
		{
			this.SetClip(path, CombineMode.Replace);
		}

		public void SetClip(Rectangle rect)
		{
			this.SetClip(rect, CombineMode.Replace);
		}

		public void SetClip(Graphics g)
		{
			this.SetClip(g, CombineMode.Replace);
		}

		public void SetClip(Graphics g, CombineMode combineMode)
		{
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipSetClipGraphics(this.nativeObject, g.NativeObject, combineMode));
		}

		public void SetClip(Rectangle rect, CombineMode combineMode)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetClipRectI(this.nativeObject, rect.X, rect.Y, rect.Width, rect.Height, combineMode));
		}

		public void SetClip(RectangleF rect, CombineMode combineMode)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetClipRect(this.nativeObject, rect.X, rect.Y, rect.Width, rect.Height, combineMode));
		}

		public void SetClip(Region region, CombineMode combineMode)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipSetClipRegion(this.nativeObject, region.NativeObject, combineMode));
		}

		public void SetClip(GraphicsPath path, CombineMode combineMode)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipSetClipPath(this.nativeObject, path.NativeObject, combineMode));
		}

		public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			IntPtr intPtr = GDIPlus.FromPointToUnManagedMemory(pts);
			GDIPlus.CheckStatus(GDIPlus.GdipTransformPoints(this.nativeObject, destSpace, srcSpace, intPtr, pts.Length));
			GDIPlus.FromUnManagedMemoryToPoint(intPtr, pts);
		}

		public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] pts)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			IntPtr intPtr = GDIPlus.FromPointToUnManagedMemoryI(pts);
			GDIPlus.CheckStatus(GDIPlus.GdipTransformPointsI(this.nativeObject, destSpace, srcSpace, intPtr, pts.Length));
			GDIPlus.FromUnManagedMemoryToPointI(intPtr, pts);
		}

		public void TranslateClip(int dx, int dy)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipTranslateClipI(this.nativeObject, dx, dy));
		}

		public void TranslateClip(float dx, float dy)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipTranslateClip(this.nativeObject, dx, dy));
		}

		public void TranslateTransform(float dx, float dy)
		{
			this.TranslateTransform(dx, dy, MatrixOrder.Prepend);
		}

		public void TranslateTransform(float dx, float dy, MatrixOrder order)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipTranslateWorldTransform(this.nativeObject, dx, dy, order));
		}

		[EditorBrowsable, MonoTODO]
		public object GetContextInfo()
		{
			throw new NotImplementedException();
		}
	}
}
