#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using SF.Sys.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.Data
{
	
	public class DataScope : IDataScope,IDisposable
    {
		IDataContextProviderFactory ProviderFactory { get; }
		RootDataContext _TopRootContext;
		ILogger Logger { get; }
		
		public DataScope(IDataContextProviderFactory ProviderFactory, ILogger<DataScope> Logger)
		{
			this.ProviderFactory = ProviderFactory;
			this.Logger = Logger;
		}
		
		public async Task<T> Use<T>(
			string Action, 
			Func<IDataContext, Task<T>> Callback, 
			DataContextFlag Flags = DataContextFlag.None,
			System.Data.IsolationLevel TransactionIsolationLevel= System.Data.IsolationLevel.Unspecified
			)
		{
			//Logger.Debug($"进入数据开始 {Action} {Flags} {TransactionIsolationLevel}");
			T result;
			if (_TopRootContext == null || !Flags.HasFlag(DataContextFlag.LightMode))
			{
				using (var provider = ProviderFactory.Create())
				{
					var trans = TransactionIsolationLevel == System.Data.IsolationLevel.Unspecified ?
						null :
						await ((IDataContextProviderExtension)provider).BeginTransaction(TransactionIsolationLevel, CancellationToken.None);
					try
					{
						using (var ctx = _TopRootContext = new RootDataContext(Action, Flags, trans, provider, _TopRootContext))
						{
							Exception error = null;
							try
							{
								result= await Callback(ctx);
							}
							catch (Exception ex)
							{
								error = ex;
								Logger.Warn(ex,$"根数据区域异常 {Action} {Flags} {TransactionIsolationLevel}");
								throw;
							}
							finally
							{
								_TopRootContext = _TopRootContext.PrevRootContext;
								await ctx.EndUse(error,Logger);
							}
						}
					}
					finally
					{
						if (trans != null)
							trans.Dispose();
					}

				}
			}
			else
			{
				if (TransactionIsolationLevel != System.Data.IsolationLevel.Unspecified)
					throw new ArgumentException("LightMode数据上下文不支持下层事务");
				using (var ctx = _TopRootContext.PushChildContext(Action))
					try
					{
						result= await Callback(ctx);
					}
					finally
					{
						_TopRootContext.PopChildContext(ctx);
					}
			}
			//Logger.Debug($"数据区域结束 {Action} {Flags} {TransactionIsolationLevel}");
			return result;

		}

		public void Dispose()
		{
			if (_TopRootContext != null)
				throw new InvalidOperationException("存在未释放的数据上下文");
		}
	}
}
