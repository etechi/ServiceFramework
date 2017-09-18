﻿using System;
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
	public class LogService : ILogService, IDisposable
	{
		private readonly Dictionary<string, LoggerCollection> _loggers = new Dictionary<string, LoggerCollection>(StringComparer.Ordinal);

		private readonly List<ILoggerProvider> _globalProviders = new List<ILoggerProvider>();
		private readonly List<ILoggerProvider> _scopedProviders = new List<ILoggerProvider>();

		private readonly object _sync = new object();

		private volatile bool _disposed;

		public ILogMessageFactory LogMessageFactory { get; }
		public LogService(ILogMessageFactory LogMessageFactory=null)
		{
			Ensure.NotNull(LogMessageFactory, nameof(LogMessageFactory));
			this.LogMessageFactory = LogMessageFactory ?? DefaultLogMessageFactory.Instance;
		}
		public ILogger GetLogger(string categoryName)
		{
			if (this.CheckDisposed())
				throw new ObjectDisposedException("LoggerFactory");
			LoggerCollection logger;
			lock (_sync)
			{
				if (!this._loggers.TryGetValue(categoryName, out logger))
				{
					logger = new LoggerCollection(this, categoryName, false);
					this._loggers[categoryName] = logger;
				}
			}
			return logger;
		}
		public ILogger CreateScopedLogger(string categoryName)
		{
			return new LoggerCollection(this, categoryName, true);
		}


		public void AddProvider(ILoggerProvider provider)
		{
			if (this.CheckDisposed())
				throw new ObjectDisposedException("LoggerFactory");
			object sync = this._sync;
			lock (sync)
			{
				if (provider.Scoped)
					_scopedProviders.Add(provider);
				else
				{
					_globalProviders.Add(provider);
					foreach (var p in _loggers)
					{
						p.Value.AddProvider(provider);
					}
				}
			}
		}

		internal IProviderLogger[] CreateLoggers(bool scoped,string Name)
		{
			var providers = scoped ? _scopedProviders : _globalProviders;
			if (providers.Count == 0)
				return null;
			var loggers = new IProviderLogger[providers.Count];
			for (int i = 0; i < providers.Count; i++)
			{
				loggers[i] = providers[i].CreateLogger(Name);
			}
			return loggers;
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
				if (provider.Scoped)
					_scopedProviders.Remove(provider);
				else
				{
					_globalProviders.Remove(provider);
					foreach (var p in _loggers)
						p.Value.RemoveProvider(provider);
				}
			}
		}
		public void Dispose()
		{
			if (!this._disposed)
			{
				this._disposed = true;
				foreach (var p in _globalProviders)
				{
					try
					{
						p.Dispose();
					}
					catch { }
				}
				foreach (var p in _scopedProviders)
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