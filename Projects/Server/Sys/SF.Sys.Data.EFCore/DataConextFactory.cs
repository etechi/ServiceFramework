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

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Data.Common;
using SF.Sys.Data;
using Microsoft.EntityFrameworkCore.Internal;

namespace SF.Sys.Data.EntityFrameworkCore
{
	
	public class DataContextProviderFactory :
		IDataContextProviderFactory
	{
		Func<DbContext> DbContextCreator { get; }
		public DataContextProviderFactory(Func<DbContext> DbContextCreator)
		{
			this.DbContextCreator = DbContextCreator;
		}
		public IDataContextProvider Create()
		{
			return new EFDbContext(DbContextCreator());
		}
	}
	public class PoolDataContextFactory<TContext> :
		IDataContextProviderFactory
		where TContext:DbContext
	{
		DbContextPool<TContext> Pool { get; }
		public PoolDataContextFactory(DbContextPool<TContext> Pool)
		{
			this.Pool = Pool;
		}
		public IDataContextProvider Create()
		{
			return new EFDbContext(Pool.Rent());
		}
	}
}
