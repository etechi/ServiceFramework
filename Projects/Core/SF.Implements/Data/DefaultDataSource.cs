using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data
{
	public class DataSourceConfig
	{
		[Comment("数据提供者")]
		[Required]
		public string Provider { get; set; }

		[Comment("连接字符串")]
		[Required]
		public string ConnectionString { get; set; }
	}
	public class DefaultDataSource : IDataSource
	{
		DbProviderFactory Factory { get; }
		DataSourceConfig Config { get; }
		public DefaultDataSource(DataSourceConfig Config)
		{
			if (Config == null)
			{
				var conn = System.Configuration.ConfigurationManager.ConnectionStrings["default"];
				Config = new DataSourceConfig
				{
					ConnectionString = conn.ConnectionString,
					Provider = conn.ProviderName
				};
			}
			this.Config = Config;
			Factory = DbProviderFactories.GetFactory(Config.Provider);
		}
		public DbConnection Connect()
		{
			var connection = Factory.CreateConnection();
			connection.ConnectionString = Config.ConnectionString;
			return connection;
		}
	}
}
