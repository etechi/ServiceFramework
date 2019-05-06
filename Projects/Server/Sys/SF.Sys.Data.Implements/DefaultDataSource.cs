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
using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace System.Data.Debug
{
}
namespace SF.Sys.Data
{
	public class DataSourceConfig
	{
		/// <summary>
		/// 数据提供者
		/// </summary>
		[Required]
		public string Provider { get; set; }

		/// <summary>
		/// 连接字符串
		/// </summary>
		[Required]
		public string ConnectionString { get; set; }

        public bool? UseRowNumberForPaging { get; set; }
        public bool? UseSqlLog{ get; set; }
    }
	
	public class DefaultDataSource : IDataSource
	{
		DbProviderFactory Factory { get; }
		DataSourceConfig Config { get; }
		//static DefaultDataSource()
		//{
		//	try
		//	{
		//		var dataSet = ConfigurationManager.GetSection("system.data") as System.Data.DataSet;
		//		dataSet.Tables[0].Rows.Add(
		//			"Debug Data Provider"
		//			, ".Net Framework Data Provider for Debug"
		//			, "System.Data.Debug"
		//			, "System.Data.Debug.DebugDbProviderFactory, SF.Implements");
		//	}
		//	catch (System.Data.ConstraintException) { }
		//}
		public DefaultDataSource(DataSourceConfig Config)
		{
			//if (Config == null)
			//{
			//	var conn = System.Configuration.ConfigurationManager.ConnectionStrings["default"];
			//	Config = new DataSourceConfig
			//	{
			//		ConnectionString = conn.ConnectionString,
			//		Provider = conn.ProviderName
			//	};
			//}
			this.Config = Config;
			Factory = System.Data.SqlClient.SqlClientFactory.Instance;
			//var f=System.Data.SqlClient.SqlClientFactory.Instance.CreateConnection()
			
		}
		public string ConnectionString=> Config.ConnectionString;
        public bool? UseRowNumberForPaging => Config.UseRowNumberForPaging;
        public bool? UseSqlLog => Config.UseSqlLog;
        public DbConnection Connect()
		{
			var connection = Factory.CreateConnection();
			connection.ConnectionString = Config.ConnectionString;
			//connection.ConnectionTimeout = 10000 + (connId++);
			connection.Disposed += Connection_Disposed;
			//connection.
			return connection;
			//return new System.Data.Debug.DebugDbConnection(connection, new System.Data.Debug.DebugDbProviderFactory(Factory));
		}

		private void Connection_Disposed(object sender, EventArgs e)
		{

		}
	}
}
