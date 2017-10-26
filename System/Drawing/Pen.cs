using System;
using System.ComponentModel;
using iTextSharp.Drawing.Drawing2D;

namespace iTextSharp.Drawing
{
	public sealed class Pen : MarshalByRefObject, ICloneable, IDisposable
	{
		internal IntPtr nativeObject;

		internal bool isModifiable = true;

		private Color color;

		private CustomLineCap startCap;

		private CustomLineCap endCap;

		[MonoLimitation("Libgdiplus doesn't use this property for rendering")]
		public PenAlignment Alignment
		{
			get
			{
				PenAlignment result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPenMode(this.nativeObject, out result));
				return result;
			}
			set
			{
				if (value < PenAlignment.Center || value > PenAlignment.Right)
				{
					throw new InvalidEnumArgumentException("Alignment", (int)value, typeof(PenAlignment));
				}
				if (this.isModifiable)
				{
					GDIPlus.CheckStatus(GDIPlus.GdipSetPenMode(this.nativeObject, value));
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public Brush Brush
		{
			get
			{
				IntPtr ptr;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPenBrushFill(this.nativeObject, out ptr));
				return new SolidBrush(ptr);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Brush");
				}
				if (!this.isModifiable)
				{
					throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
				}
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenBrushFill(this.nativeObject, value.nativeObject));
				this.color = Color.Empty;
			}
		}

		public Color Color
		{
			get
			{
				if (this.color.Equals(Color.Empty))
				{
					int num;
					GDIPlus.CheckStatus(GDIPlus.GdipGetPenColor(this.nativeObject, out num));
					this.color = Color.FromArgb(num);
				}
				return this.color;
			}
			set
			{
				if (!this.isModifiable)
				{
					throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
				}
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenColor(this.nativeObject, value.ToArgb()));
				this.color = value;
			}
		}

		public float[] CompoundArray
		{
			get
			{
				int num;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPenCompoundCount(this.nativeObject, out num));
				float[] array = new float[num];
				GDIPlus.CheckStatus(GDIPlus.GdipGetPenCompoundArray(this.nativeObject, array, num));
				return array;
			}
			set
			{
				if (!this.isModifiable)
				{
					throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
				}
				if (value.Length < 2)
				{
					throw new ArgumentException("Invalid parameter.");
				}
				for (int i = 0; i < value.Length; i++)
				{
					float num = value[i];
					if (num < 0f || num > 1f)
					{
						throw new ArgumentException("Invalid parameter.");
					}
				}
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenCompoundArray(this.nativeObject, value, value.Length));
			}
		}

		public CustomLineCap CustomEndCap
		{
			get
			{
				return this.endCap;
			}
			set
			{
				if (this.isModifiable)
				{
					GDIPlus.CheckStatus(GDIPlus.GdipSetPenCustomEndCap(this.nativeObject, value.nativeObject));
					this.endCap = value;
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public CustomLineCap CustomStartCap
		{
			get
			{
				return this.startCap;
			}
			set
			{
				if (this.isModifiable)
				{
					GDIPlus.CheckStatus(GDIPlus.GdipSetPenCustomStartCap(this.nativeObject, value.nativeObject));
					this.startCap = value;
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public DashCap DashCap
		{
			get
			{
				DashCap result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPenDashCap197819(this.nativeObject, out result));
				return result;
			}
			set
			{
				if (value < DashCap.Flat || value > DashCap.Triangle)
				{
					throw new InvalidEnumArgumentException("DashCap", (int)value, typeof(DashCap));
				}
				if (this.isModifiable)
				{
					GDIPlus.CheckStatus(GDIPlus.GdipSetPenDashCap197819(this.nativeObject, value));
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public float DashOffset
		{
			get
			{
				float result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPenDashOffset(this.nativeObject, out result));
				return result;
			}
			set
			{
				if (this.isModifiable)
				{
					GDIPlus.CheckStatus(GDIPlus.GdipSetPenDashOffset(this.nativeObject, value));
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public float[] DashPattern
		{
			get
			{
				int num;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPenDashCount(this.nativeObject, out num));
				float[] array;
				if (num > 0)
				{
					array = new float[num];
					GDIPlus.CheckStatus(GDIPlus.GdipGetPenDashArray(this.nativeObject, array, num));
				}
				else if (this.DashStyle == DashStyle.Custom)
				{
					array = new float[]
					{
						1f
					};
				}
				else
				{
					array = new float[0];
				}
				return array;
			}
			set
			{
				if (!this.isModifiable)
				{
					throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
				}
				if (value.Length == 0)
				{
					throw new ArgumentException("Invalid parameter.");
				}
				for (int i = 0; i < value.Length; i++)
				{
					if (value[i] <= 0f)
					{
						throw new ArgumentException("Invalid parameter.");
					}
				}
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenDashArray(this.nativeObject, value, value.Length));
			}
		}

		public DashStyle DashStyle
		{
			get
			{
				DashStyle result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPenDashStyle(this.nativeObject, out result));
				return result;
			}
			set
			{
				if (value < DashStyle.Solid || value > DashStyle.Custom)
				{
					throw new InvalidEnumArgumentException("DashStyle", (int)value, typeof(DashStyle));
				}
				if (this.isModifiable)
				{
					GDIPlus.CheckStatus(GDIPlus.GdipSetPenDashStyle(this.nativeObject, value));
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public LineCap StartCap
		{
			get
			{
				LineCap result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPenStartCap(this.nativeObject, out result));
				return result;
			}
			set
			{
				if (value < LineCap.Flat || value > LineCap.Custom)
				{
					throw new InvalidEnumArgumentException("StartCap", (int)value, typeof(LineCap));
				}
				if (this.isModifiable)
				{
					GDIPlus.CheckStatus(GDIPlus.GdipSetPenStartCap(this.nativeObject, value));
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public LineCap EndCap
		{
			get
			{
				LineCap result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPenEndCap(this.nativeObject, out result));
				return result;
			}
			set
			{
				if (value < LineCap.Flat || value > LineCap.Custom)
				{
					throw new InvalidEnumArgumentException("EndCap", (int)value, typeof(LineCap));
				}
				if (this.isModifiable)
				{
					GDIPlus.CheckStatus(GDIPlus.GdipSetPenEndCap(this.nativeObject, value));
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public LineJoin LineJoin
		{
			get
			{
				LineJoin result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPenLineJoin(this.nativeObject, out result));
				return result;
			}
			set
			{
				if (value < LineJoin.Miter || value > LineJoin.MiterClipped)
				{
					throw new InvalidEnumArgumentException("LineJoin", (int)value, typeof(LineJoin));
				}
				if (this.isModifiable)
				{
					GDIPlus.CheckStatus(GDIPlus.GdipSetPenLineJoin(this.nativeObject, value));
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public float MiterLimit
		{
			get
			{
				float result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPenMiterLimit(this.nativeObject, out result));
				return result;
			}
			set
			{
				if (this.isModifiable)
				{
					GDIPlus.CheckStatus(GDIPlus.GdipSetPenMiterLimit(this.nativeObject, value));
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public PenType PenType
		{
			get
			{
				PenType result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPenFillType(this.nativeObject, out result));
				return result;
			}
		}

		public Matrix Transform
		{
			get
			{
				Matrix matrix = new Matrix();
				GDIPlus.CheckStatus(GDIPlus.GdipGetPenTransform(this.nativeObject, matrix.nativeMatrix));
				return matrix;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Transform");
				}
				if (this.isModifiable)
				{
					GDIPlus.CheckStatus(GDIPlus.GdipSetPenTransform(this.nativeObject, value.nativeMatrix));
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public float Width
		{
			get
			{
				float result;
				GDIPlus.CheckStatus(GDIPlus.GdipGetPenWidth(this.nativeObject, out result));
				return result;
			}
			set
			{
				if (this.isModifiable)
				{
					GDIPlus.CheckStatus(GDIPlus.GdipSetPenWidth(this.nativeObject, value));
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		internal Pen(IntPtr p)
		{
			this.nativeObject = p;
		}

		public Pen(Brush brush) : this(brush, 1f)
		{
		}

		public Pen(Color color) : this(color, 1f)
		{
		}

		public Pen(Brush brush, float width)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCreatePen2(brush.nativeObject, width, GraphicsUnit.World, out this.nativeObject));
			this.color = Color.Empty;
		}

		public Pen(Color color, float width)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCreatePen1(color.ToArgb(), width, GraphicsUnit.World, out this.nativeObject));
			this.color = color;
		}

		public object Clone()
		{
			IntPtr p;
			GDIPlus.CheckStatus(GDIPlus.GdipClonePen(this.nativeObject, out p));
			return new Pen(p)
			{
				startCap = this.startCap,
				endCap = this.endCap
			};
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing && !this.isModifiable)
			{
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
			if (this.nativeObject != IntPtr.Zero)
			{
				Status arg_43_0 = GDIPlus.GdipDeletePen(this.nativeObject);
				this.nativeObject = IntPtr.Zero;
				GDIPlus.CheckStatus(arg_43_0);
			}
		}

		~Pen()
		{
			this.Dispose(false);
		}

		public void MultiplyTransform(Matrix matrix)
		{
			this.MultiplyTransform(matrix, MatrixOrder.Prepend);
		}

		public void MultiplyTransform(Matrix matrix, MatrixOrder order)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipMultiplyPenTransform(this.nativeObject, matrix.nativeMatrix, order));
		}

		public void ResetTransform()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipResetPenTransform(this.nativeObject));
		}

		public void RotateTransform(float angle)
		{
			this.RotateTransform(angle, MatrixOrder.Prepend);
		}

		public void RotateTransform(float angle, MatrixOrder order)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipRotatePenTransform(this.nativeObject, angle, order));
		}

		public void ScaleTransform(float sx, float sy)
		{
			this.ScaleTransform(sx, sy, MatrixOrder.Prepend);
		}

		public void ScaleTransform(float sx, float sy, MatrixOrder order)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipScalePenTransform(this.nativeObject, sx, sy, order));
		}

		public void SetLineCap(LineCap startCap, LineCap endCap, DashCap dashCap)
		{
			if (this.isModifiable)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenLineCap197819(this.nativeObject, startCap, endCap, dashCap));
				return;
			}
			throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
		}

		public void TranslateTransform(float dx, float dy)
		{
			this.TranslateTransform(dx, dy, MatrixOrder.Prepend);
		}

		public void TranslateTransform(float dx, float dy, MatrixOrder order)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipTranslatePenTransform(this.nativeObject, dx, dy, order));
		}
	}
}
