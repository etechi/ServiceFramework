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

using SF.Sys.Data;
using SF.Sys.Logging;
using SF.Sys.Services;
using System;

namespace SF.Sys.Services
{

	public static class DataContextCollectionExtension
	{
		public static IServiceCollection AddDataContext(this IServiceCollection sc, string ConnectionString)
			=> sc.AddDataContext(sp => new DataSourceConfig { ConnectionString = ConnectionString });

		public static IServiceCollection AddDataContext(this IServiceCollection sc,Func<IServiceProvider, DataSourceConfig> Config)
		{
			sc.AddSingleton<IDataSource>(sp=>new DefaultDataSource(Config(sp)));
			//sc.AddScoped(sp => sp.Resolve<IDataSource>().Connect());

			sc.AddScoped<SF.Sys.Data.IDataContext, SF.Sys.Data.DataContext>();
			sc.Add(typeof(SF.Sys.Data.IDataSet<>),typeof(SF.Sys.Data.DataSet<>),ServiceImplementLifetime.Scoped);
			sc.AddScoped<SF.Sys.Data.ITransactionScopeManager, TransactionScopeManager>();
			return sc;
		}
	}
	

}
