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
	internal class Logger : ILogger
	{
		private class Scope : IDisposable
		{
			private bool _isDisposed;

			private IDisposable _disposable0;

			private IDisposable _disposable1;

			private readonly IDisposable[] _disposable;

			public Scope(int count)
			{
				if (count > 2)
				{
					this._disposable = new IDisposable[count - 2];
				}
			}

			public void SetDisposable(int index, IDisposable disposable)
			{
				if (index == 0)
				{
					this._disposable0 = disposable;
					return;
				}
				if (index == 1)
				{
					this._disposable1 = disposable;
					return;
				}
				this._disposable[index - 2] = disposable;
			}

			public void Dispose()
			{
				if (!this._isDisposed)
				{
					if (this._disposable0 != null)
					{
						this._disposable0.Dispose();
					}
					if (this._disposable1 != null)
					{
						this._disposable1.Dispose();
					}
					if (this._disposable != null)
					{
						int num = this._disposable.Length;
						for (int num2 = 0; num2 != num; num2++)
						{
							if (this._disposable[num2] != null)
							{
								this._disposable[num2].Dispose();
							}
						}
					}
					this._isDisposed = true;
				}
			}

			internal void Add(IDisposable disposable)
			{
				throw new NotImplementedException();
			}
		}

		private readonly LoggerService _loggerService;

		private readonly string _name;

		private IProviderLogger[] _loggers;

		public Logger(LoggerService loggerService, string name)
		{
			this._loggerService = loggerService;
			this._name = name;
			ILoggerProvider[] providers = loggerService.GetProviders();
			if (providers.Length != 0)
			{
				this._loggers = new IProviderLogger[providers.Length];
				for (int i = 0; i < providers.Length; i++)
				{
					this._loggers[i] = providers[i].CreateLogger(name);
				}
			}
		}

		public void Write<TState>(LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (this._loggers == null)
				return;
			List<Exception> list = null;
			ILogger[] loggers = this._loggers;
			for (int i = 0; i < loggers.Length; i++)
			{
				ILogger logger = loggers[i];
				try
				{
					logger.Write<TState>(logLevel,  state, exception, formatter);
				}
				catch (Exception item)
				{
					if (list == null)
					{
						list = new List<Exception>();
					}
					list.Add(item);
				}
			}
			if (list != null && list.Count > 0)
			{
				throw new AggregateException("An error occurred while writing to logger(s).", list);
			}
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			if (this._loggers == null)
				return false;
			List<Exception> list = null;
			var loggers = this._loggers;
			foreach(var logger in loggers)
			{
				try
				{
					if (logger.IsEnabled(logLevel))
					{
						return true;
					}
				}
				catch (Exception item)
				{
					if (list == null)
					{
						list = new List<Exception>();
					}
					list.Add(item);
				}
			}
			if (list != null && list.Count > 0)
			{
				throw new AggregateException("An error occurred while writing to logger(s).", list);
			}
			return false;
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			var loggers = _loggers;
			if (loggers == null)
				return NullScope.Instance;
			
			if (loggers.Length == 1)
				return loggers[0].BeginScope<TState>(state);

			var scope = new Scope(loggers.Length);
			List<Exception> list = null;
			for (int i = 0; i < loggers.Length; i++)
			{
				try
				{
					IDisposable disposable = loggers[i].BeginScope<TState>(state);
					scope.SetDisposable(i, disposable);
				}
				catch (Exception item)
				{
					if (list == null)
					{
						list = new List<Exception>();
					}
					list.Add(item);
				}
			}
			if (list != null && list.Count > 0)
			{
				throw new AggregateException("An error occurred while writing to logger(s).", list);
			}
			return scope;
		}

		internal void AddProvider(ILoggerProvider provider)
		{
			var logger = provider.CreateLogger(this._name);
			
			_loggers = (_loggers?? Array.Empty<IProviderLogger>()).Concat(new[] { logger });
		}
		internal void RemoveProvider(ILoggerProvider provider)
		{
			_loggers = _loggers.Where(l => l.Provider != provider).ToArray();
		}
	}

}
