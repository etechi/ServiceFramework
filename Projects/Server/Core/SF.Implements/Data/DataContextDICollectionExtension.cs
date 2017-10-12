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

using SF.Data;
using SF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
namespace SF.Core.ServiceManagement
{
	
	public static class DataContextCollectionExtension
	{
		public static IServiceCollection AddDataContext(this IServiceCollection sc, DataSourceConfig Config)
		{
			sc.AddSingleton<IDataSource>(new DefaultDataSource(Config));
			sc.AddScoped(sp => sp.Resolve<IDataSource>().Connect());

			sc.AddScoped<SF.Data.IDataContext, SF.Data.DataContext>();
			sc.Add(typeof(SF.Data.IDataSet<>),typeof(SF.Data.DataSet<>),ServiceImplementLifetime.Scoped);
			sc.AddScoped<SF.Data.ITransactionScopeManager, TransactionScopeManager>();
			return sc;
		}
	}
	

}
