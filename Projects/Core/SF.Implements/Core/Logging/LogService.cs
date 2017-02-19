using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using SF.Core.NetworkService.Metadata;
using System.Text;
using System.Threading.Tasks;
using SF.Core.ServiceFeatures;

namespace SF.Core.Logging
{
	public class LoggerService : ILogService, IDisposable
	{
		private readonly Dictionary<string, Logger> _loggers = new Dictionary<string, Logger>(StringComparer.Ordinal);

		private readonly List<ILoggerProvider> _providers = new List<ILoggerProvider>();

		private readonly object _sync = new object();

		private volatile bool _disposed;

		public ILogger GetLogger(string categoryName)
		{
			if (this.CheckDisposed())
				throw new ObjectDisposedException("LoggerFactory");
			Logger logger;
			lock (_sync)
			{
				if (!this._loggers.TryGetValue(categoryName, out logger))
				{
					logger = new Logger(this, categoryName);
					this._loggers[categoryName] = logger;
				}
			}
			return logger;
		}

		public void AddProvider(ILoggerProvider provider)
		{
			if (this.CheckDisposed())
				throw new ObjectDisposedException("LoggerFactory");
			object sync = this._sync;
			lock (sync)
			{
				_providers.Add(provider);
				foreach (var p in _loggers)
				{
					p.Value.AddProvider(provider);
				}
			}
		}

		internal ILoggerProvider[] GetProviders()
		{
			return _providers.ToArray();
		}

		/// <summary>
		/// Check if the factory has been disposed.
		/// </summary>
		/// <returns>True when <see cref="M:Microsoft.Extensions.Logging.LoggerFactory.Dispose" /> as been called</returns>
		protected virtual bool CheckDisposed()
		{
			return this._disposed;
		}

		public void RemoveProvider(ILoggerProvider provider)
		{
			if (this.CheckDisposed())
				throw new ObjectDisposedException("LoggerFactory");
			object sync = this._sync;
			lock (sync)
			{
				_providers.Remove(provider);
				foreach (var p in _loggers)
					p.Value.RemoveProvider(provider);
			}
		}
		public void Dispose()
		{
			if (!this._disposed)
			{
				this._disposed = true;
				foreach (var p in _providers)
				{
					try
					{
						p.Dispose();
					}
					catch { }
				}
			}
		}

	}

}
