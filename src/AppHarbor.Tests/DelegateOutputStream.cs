using System;
using System.IO;

namespace AppHarbor.Tests
{
	public class DelegateOutputStream : MemoryStream
	{
		private readonly Action<MemoryStream> _beforeDispose;
		private bool _hasDisposed;

		public DelegateOutputStream(Action<MemoryStream> beforeDispose)
		{
			_beforeDispose = beforeDispose;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_hasDisposed)
			{
				_hasDisposed = true;
				_beforeDispose(this);
			}

			base.Dispose(disposing);
		}
	}
}
