﻿#region Apache License Version 2.0
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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SF.Sys.Data;
using SF.Sys.Data.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using System.Data.Common;

namespace SF.Sys.Services
{
	public static class EFCoreServiceCollectioExtension
	{

		public static IServiceCollection AddEFCoreDataContextFactory(
			this IServiceCollection sc,
			Func<IServiceProvider, DbContext> DbContextCreator)
		{
			sc.AddSingleton<IDataContextProviderFactory>(x =>
				new DataContextProviderFactory(
					() => 
					DbContextCreator(x)
					)
				);
			return sc;
		}
		public static IServiceCollection AddEFCorePoolDataContextFactory<TDbContext>(
			this IServiceCollection sc
			)
			where TDbContext:DbContext
		{
			sc.AddSingleton<IDataContextProviderFactory, PoolDataContextFactory<TDbContext>>();
			return sc;
		}
		public static IServiceCollection AddEFCorePoolDataContextFactory(
		   this IServiceCollection sc
		   )
		{
			sc.AddSingleton<IDataContextProviderFactory, PoolDataContextFactory<DbContext>>();
			return sc;
		}
	}
	

}
