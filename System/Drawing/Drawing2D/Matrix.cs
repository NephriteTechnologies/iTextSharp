using System;
using System.Runtime.InteropServices;

namespace iTextSharp.Drawing.Drawing2D
{
	public sealed class Matrix : MarshalByRefObject, IDisposable
	{
		internal IntPtr nativeMatrix;

		public float[] Elements
		{
			get
			{
				float[] array = new float[6];
				IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * 6);
				try
				{
					GDIPlus.CheckStatus(GDIPlus.GdipGetMatrixElements(this.nativeMatrix, intPtr));
					Marshal.Copy(intPtr, array, 0, 6);
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
				return array;
			}
		}

		public bool IsIdentity
		{
			get
			{
				bool result;
				GDIPlus.CheckStatus(GDIPlus.GdipIsMatrixIdentity(this.nativeMatrix, out result));
				return result;
			}
		}

		public bool IsInvertible
		{
			get
			{
				bool result;
				GDIPlus.CheckStatus(GDIPlus.GdipIsMatrixInvertible(this.nativeMatrix, out result));
				return result;
			}
		}

		public float OffsetX
		{
			get
			{
				return this.Elements[4];
			}
		}

		public float OffsetY
		{
			get
			{
				return this.Elements[5];
			}
		}

		internal IntPtr NativeObject
		{
			get
			{
				return this.nativeMatrix;
			}
			set
			{
				this.nativeMatrix = value;
			}
		}

		internal Matrix(IntPtr ptr)
		{
			this.nativeMatrix = ptr;
		}

		public Matrix()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCreateMatrix(out this.nativeMatrix));
		}

		public Matrix(Rectangle rect, Point[] plgpts)
		{
			if (plgpts == null)
			{
				throw new ArgumentNullException("plgpts");
			}
			if (plgpts.Length != 3)
			{
				throw new ArgumentException("plgpts");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCreateMatrix3I(ref rect, plgpts, out this.nativeMatrix));
		}

		public Matrix(RectangleF rect, PointF[] plgpts)
		{
			if (plgpts == null)
			{
				throw new ArgumentNullException("plgpts");
			}
			if (plgpts.Length != 3)
			{
				throw new ArgumentException("plgpts");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipCreateMatrix3(ref rect, plgpts, out this.nativeMatrix));
		}

		public Matrix(float m11, float m12, float m21, float m22, float dx, float dy)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipCreateMatrix2(m11, m12, m21, m22, dx, dy, out this.nativeMatrix));
		}

		public Matrix Clone()
		{
			IntPtr ptr;
			GDIPlus.CheckStatus(GDIPlus.GdipCloneMatrix(this.nativeMatrix, out ptr));
			return new Matrix(ptr);
		}

		public void Dispose()
		{
			if (this.nativeMatrix != IntPtr.Zero)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipDeleteMatrix(this.nativeMatrix));
				this.nativeMatrix = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		public override bool Equals(object obj)
		{
			Matrix matrix = obj as Matrix;
			if (matrix != null)
			{
				bool result;
				GDIPlus.CheckStatus(GDIPlus.GdipIsMatrixEqual(this.nativeMatrix, matrix.nativeMatrix, out result));
				return result;
			}
			return false;
		}

		~Matrix()
		{
			this.Dispose();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void Invert()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipInvertMatrix(this.nativeMatrix));
		}

		public void Multiply(Matrix matrix)
		{
			this.Multiply(matrix, MatrixOrder.Prepend);
		}

		public void Multiply(Matrix matrix, MatrixOrder order)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException("matrix");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipMultiplyMatrix(this.nativeMatrix, matrix.nativeMatrix, order));
		}

		public void Reset()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetMatrixElements(this.nativeMatrix, 1f, 0f, 0f, 1f, 0f, 0f));
		}

		public void Rotate(float angle)
		{
			this.Rotate(angle, MatrixOrder.Prepend);
		}

		public void Rotate(float angle, MatrixOrder order)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipRotateMatrix(this.nativeMatrix, angle, order));
		}

		public void RotateAt(float angle, PointF point)
		{
			this.RotateAt(angle, point, MatrixOrder.Prepend);
		}

		public void RotateAt(float angle, PointF point, MatrixOrder order)
		{
			if (order < MatrixOrder.Prepend || order > MatrixOrder.Append)
			{
				throw new ArgumentException("order");
			}
			angle *= 0.0174532924f;
			float num = (float)Math.Cos((double)angle);
			float num2 = (float)Math.Sin((double)angle);
			float num3 = -point.X * num + point.Y * num2 + point.X;
			float num4 = -point.X * num2 - point.Y * num + point.Y;
			float[] elements = this.Elements;
			Status status;
			if (order == MatrixOrder.Prepend)
			{
				status = GDIPlus.GdipSetMatrixElements(this.nativeMatrix, num * elements[0] + num2 * elements[2], num * elements[1] + num2 * elements[3], -num2 * elements[0] + num * elements[2], -num2 * elements[1] + num * elements[3], num3 * elements[0] + num4 * elements[2] + elements[4], num3 * elements[1] + num4 * elements[3] + elements[5]);
			}
			else
			{
				status = GDIPlus.GdipSetMatrixElements(this.nativeMatrix, elements[0] * num + elements[1] * -num2, elements[0] * num2 + elements[1] * num, elements[2] * num + elements[3] * -num2, elements[2] * num2 + elements[3] * num, elements[4] * num + elements[5] * -num2 + num3, elements[4] * num2 + elements[5] * num + num4);
			}
			GDIPlus.CheckStatus(status);
		}

		public void Scale(float scaleX, float scaleY)
		{
			this.Scale(scaleX, scaleY, MatrixOrder.Prepend);
		}

		public void Scale(float scaleX, float scaleY, MatrixOrder order)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipScaleMatrix(this.nativeMatrix, scaleX, scaleY, order));
		}

		public void Shear(float shearX, float shearY)
		{
			this.Shear(shearX, shearY, MatrixOrder.Prepend);
		}

		public void Shear(float shearX, float shearY, MatrixOrder order)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipShearMatrix(this.nativeMatrix, shearX, shearY, order));
		}

		public void TransformPoints(Point[] pts)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipTransformMatrixPointsI(this.nativeMatrix, pts, pts.Length));
		}

		public void TransformPoints(PointF[] pts)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipTransformMatrixPoints(this.nativeMatrix, pts, pts.Length));
		}

		public void TransformVectors(Point[] pts)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipVectorTransformMatrixPointsI(this.nativeMatrix, pts, pts.Length));
		}

		public void TransformVectors(PointF[] pts)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipVectorTransformMatrixPoints(this.nativeMatrix, pts, pts.Length));
		}

		public void Translate(float offsetX, float offsetY)
		{
			this.Translate(offsetX, offsetY, MatrixOrder.Prepend);
		}

		public void Translate(float offsetX, float offsetY, MatrixOrder order)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipTranslateMatrix(this.nativeMatrix, offsetX, offsetY, order));
		}

		public void VectorTransformPoints(Point[] pts)
		{
			this.TransformVectors(pts);
		}
	}
}
