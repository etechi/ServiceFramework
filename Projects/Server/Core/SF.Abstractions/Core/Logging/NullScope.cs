using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.Logging
{

	/// <summary>
	/// An empty scope without any logic
	/// </summary>
	public class NullScope : IDisposable
	{
		public static NullScope Instance { get; } = new NullScope();

		private NullScope()
		{
		}
		/// <inheritdoc />
		public void Dispose()
		{
		}

	}
}
