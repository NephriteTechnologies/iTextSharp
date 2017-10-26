using System;

namespace iTextSharp.Drawing
{
	public sealed class BufferedGraphics : IDisposable
	{
		private Rectangle size;

		private Bitmap membmp;

		private Graphics target;

		private Graphics source;

		public Graphics Graphics
		{
			get
			{
				if (this.source == null)
				{
					this.source = Graphics.FromImage(this.membmp);
				}
				return this.source;
			}
		}

		private BufferedGraphics()
		{
		}

		internal BufferedGraphics(Graphics targetGraphics, Rectangle targetRectangle)
		{
			this.size = targetRectangle;
			this.target = targetGraphics;
			this.membmp = new Bitmap(this.size.Width, this.size.Height);
		}

		~BufferedGraphics()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
			if (this.membmp != null)
			{
				this.membmp.Dispose();
				this.membmp = null;
			}
			if (this.source != null)
			{
				this.source.Dispose();
				this.source = null;
			}
			this.target = null;
		}

		public void Render()
		{
			this.Render(this.target);
		}

		public void Render(Graphics target)
		{
			if (target == null)
			{
				return;
			}
			target.DrawImage(this.membmp, this.size);
		}

		[MonoTODO("The targetDC parameter has no equivalent in libgdiplus.")]
		public void Render(IntPtr targetDC)
		{
			throw new NotImplementedException();
		}
	}
}
