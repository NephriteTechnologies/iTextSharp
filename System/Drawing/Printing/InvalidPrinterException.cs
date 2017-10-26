using System;
using System.Runtime.Serialization;

namespace iTextSharp.Drawing.Printing
{
	[Serializable]
	public class InvalidPrinterException : SystemException
	{
		public InvalidPrinterException(PrinterSettings settings) : base(InvalidPrinterException.GetMessage(settings))
		{
		}

		protected InvalidPrinterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
		}

		private static string GetMessage(PrinterSettings settings)
		{
			if (settings.PrinterName == null || settings.PrinterName == string.Empty)
			{
				return "No Printers Installed";
			}
			return string.Format("Tried to access printer '{0}' with invalid settings.", settings.PrinterName);
		}
	}
}
