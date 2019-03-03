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

using SF.Sys.Linq;
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
		static AsyncLocal<DataContext> TopRootContext { get; }=new AsyncLocal<DataContext>();
		ILogger Logger { get; }
		volatile int _ContextIdSeed;
		static volatile int _IdSeed;

        int Id { get; }
        public DataScope(IDataContextProviderFactory ProviderFactory, ILogger<DataScope> Logger)
		{
			this.ProviderFactory = ProviderFactory;
			this.Logger = Logger;
            this.Id = Interlocked.Increment(ref _IdSeed); 
                
		}

        public async Task<T> Use<T>(
			string Action, 
			Func<IDataContext, Task<T>> Callback, 
			DataContextFlag Flags = DataContextFlag.None,
			System.Data.IsolationLevel TransactionIsolationLevel= System.Data.IsolationLevel.Unspecified
			)
		{
            //Logger.Debug($"{Id} 数据区域开始 {Action} {Flags} {TransactionIsolationLevel}");

            var prevContext = TopRootContext.Value;
            using (var ctx = await DataContext.Create(
                Id, 
                Interlocked.Increment(ref _ContextIdSeed), 
                Action,
                this,
                prevContext,
                ProviderFactory, 
                Flags, 
                TransactionIsolationLevel
                ))
            {
                TopRootContext.Value = ctx;

                Exception error = null;
                try
                {
                    return  await Callback(ctx);
                }
                catch (Exception ex)
                {
                    error = ex;
                    Logger.Warn(ex, $"根数据区域异常 {Action} {Flags} {TransactionIsolationLevel}");
                    throw;
                }
                finally
                {
                    //Logger.Info($"{Id} pop {ctxDump(ctx)} {ctxDump(TopRootContext.Value)}");

                    //if (ctx != TopRootContext.Value || TopRootContext.Value.PrevRootContext != curTopRootContext)
                    //    throw new InvalidOperationException($"TopRootContext 异常: {Action} {(ctx != TopRootContext.Value ? "ctx!=top" : TopRootContext.Value.PrevRootContext != curTopRootContext ? "top.prev!=prev" : "")} MaxScope:{_IdSeed} Scope:{Id} maxContext:{_ContextIdSeed} ctx:{ctxDump(ctx)} top:{ctxDump(TopRootContext.Value)} prev:{ctxDump(curTopRootContext)}");

                    TopRootContext.Value = prevContext;

                    await ctx.EndUsing(error, Logger);
                    //Logger.Debug($"{Id} 数据区域结束 {Action} {Flags} {TransactionIsolationLevel}");
                }
            }


            /*
                T result;

            var curTopRootContext = TopRootContext.Value;
            if (curTopRootContext == null ||
                curTopRootContext.DataScope!=this ||
                TransactionIsolationLevel>(curTopRootContext.Transaction?.IsolationLevel ?? System.Data.IsolationLevel.Unspecified) ||
                ((Flags & DataContextFlag.LightMode)!= DataContextFlag.LightMode && TransactionIsolationLevel!=System.Data.IsolationLevel.Unspecified)
                )
            {
                using (var provider = ProviderFactory.Create())
                {
                    var trans = TransactionIsolationLevel == System.Data.IsolationLevel.Unspecified ?
                        null :
                        await ((IDataContextProviderExtension)provider).BeginTransaction(TransactionIsolationLevel, CancellationToken.None);
                    try
                    {
                        string ctxDump(RootDataContext x) =>
                            x == null ? "null" : ADT.Link.ToEnumerable(x, c => c.PrevRootContext).Reverse().Select(c => c.ScopeId + "." + c.ContextId).Join("/");

                        using (var ctx = TopRootContext.Value = new RootDataContext(
                            Id,
                            Interlocked.Increment(ref _ContextIdSeed),
                            Action,
                            Flags,
                            trans,
                            provider,
                            this,
                            curTopRootContext
                            ))
                        {
                            Logger.Info($"{0} push {ctxDump(ctx)}");
                            Exception error = null;
                            try
                            {
                                result = await Callback(ctx);
                            }
                            catch (Exception ex)
                            {
                                error = ex;
                                Logger.Warn(ex, $"根数据区域异常 {Action} {Flags} {TransactionIsolationLevel}");
                                throw;
                            }
                            finally
                            {
                                Logger.Info($"{Id} pop {ctxDump(ctx)} {ctxDump(TopRootContext.Value)}");

                                if (ctx != TopRootContext.Value || TopRootContext.Value.PrevRootContext != curTopRootContext)
                                    throw new InvalidOperationException($"TopRootContext 异常: {Action} {(ctx != TopRootContext.Value ? "ctx!=top" : TopRootContext.Value.PrevRootContext != curTopRootContext ? "top.prev!=prev" : "")} MaxScope:{_IdSeed} Scope:{Id} maxContext:{_ContextIdSeed} ctx:{ctxDump(ctx)} top:{ctxDump(TopRootContext.Value)} prev:{ctxDump(curTopRootContext)}");

                                TopRootContext.Value = curTopRootContext;

                                await ctx.EndUsing(error, Logger);
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
                //if (TransactionIsolationLevel != System.Data.IsolationLevel.Unspecified && (
                //    _TopRootContext.Transaction == null || _TopRootContext.Transaction.IsolationLevel != TransactionIsolationLevel)
                //    )
                //    throw new ArgumentException($"LightMode数据上下文事务隔离级别{TransactionIsolationLevel}和顶层事务隔离级别{_TopRootContext.Transaction.IsolationLevel}不一致");


                using (var ctx = curTopRootContext.PushLightContext(
                        Id,
                        Interlocked.Increment(ref _ContextIdSeed),
                        Action))
                    try
                    {
                        result = await Callback(ctx);
                    }
                    finally
                    {

                        //if (_CallLevel != prevCallLevel + 1)
                         //   throw new InvalidOperationException();
                        //_CallLevel--;
                        curTopRootContext.PopLightContext(ctx);
                    }
            }
            Logger.Debug($"{Id} 数据区域结束 {Action} {Flags} {TransactionIsolationLevel}");
            return result;
            */
		}

		public void Dispose()
		{
            //if (_TopRootContext != null)
            // throw new InvalidOperationException("存在未释放的数据上下文");
		}
	}
}
