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

using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Threading;
using System.Security;
using System.Security.Permissions;
using System.Configuration;

namespace System.Data.Debug
{
	class DebugDbTransaction : DbTransaction
	{
		public DbTransaction BaseTransaction { get; }
		public DebugDbTransaction(DbTransaction BaseTransaction, DebugDbConnection Connection)
		{
			this.BaseTransaction = BaseTransaction;
			DbConnection = Connection;
		}
		public override IsolationLevel IsolationLevel => throw new NotImplementedException();

		protected override DbConnection DbConnection { get; }

		public override void Commit()
		{
			BaseTransaction.Commit();
		}

		public override void Rollback()
		{
			BaseTransaction.Rollback();
		}

		protected override void Dispose(bool disposing)
		{
			BaseTransaction.Dispose();
			base.Dispose(disposing);
		}
	}
	class DebugDbCommand : DbCommand
	{
		DebugDbConnection _DbConnection;

		DbCommand _BaseCommand;

		public DbCommand BaseCommand => _BaseCommand;

		public override string CommandText { get => BaseCommand.CommandText; set => BaseCommand.CommandText = value; }
		public override int CommandTimeout { get => BaseCommand.CommandTimeout; set => BaseCommand.CommandTimeout = value; }
		public override CommandType CommandType { get => BaseCommand.CommandType; set => BaseCommand.CommandType = value; }
		protected override DbConnection DbConnection
		{
			get => _DbConnection;
			set
			{
				if (_DbConnection != value && BaseCommand != null)
					Disposable.Release(ref _BaseCommand);

				_DbConnection = (DebugDbConnection)value;
				if (_BaseCommand == null)
					_BaseCommand = _DbConnection.BaseConnection.CreateCommand();
				else
					_BaseCommand.Connection = _DbConnection.BaseConnection;
			}
		}

		protected override DbParameterCollection DbParameterCollection => BaseCommand.Parameters;

		protected override DbTransaction DbTransaction { get => BaseCommand.Transaction; set => BaseCommand.Transaction = value; }
		public override bool DesignTimeVisible { get; set; }
		public override UpdateRowSource UpdatedRowSource { get => BaseCommand.UpdatedRowSource; set => BaseCommand.UpdatedRowSource = value; }
		public DebugDbCommand()
		{

		}
		public DebugDbCommand(DbCommand BaseCommand, DebugDbConnection connection)
		{
			_BaseCommand = BaseCommand;
			_DbConnection = connection;
		}

		public override void Cancel()
		{
			BaseCommand.Cancel();
		}

		protected override DbParameter CreateDbParameter()
		{
			return BaseCommand.CreateParameter();
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return BaseCommand.ExecuteReader(behavior);
		}

		public override int ExecuteNonQuery()
		{
			return BaseCommand.ExecuteNonQuery();
		}

		public override object ExecuteScalar()
		{
			return BaseCommand.ExecuteScalar();
		}

		public override void Prepare()
		{
			BaseCommand.Prepare();
		}
		protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
		{
			return BaseCommand.ExecuteReaderAsync(behavior, cancellationToken);
		}
		public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
		{
			return BaseCommand.ExecuteNonQueryAsync(cancellationToken);
		}
		public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
		{
			return BaseCommand.ExecuteScalarAsync(cancellationToken);
		}

		protected override void Dispose(bool disposing)
		{
			Disposable.Release(ref _BaseCommand);
			base.Dispose(disposing);
		}
	}
	class DebugDbConnection : DbConnection
	{
		DbConnection _BaseConnection;
		public DbConnection BaseConnection
		{
			get
			{
				if (_BaseConnection == null)
					_BaseConnection = ProviderFactory.BaseFactory.CreateConnection();
				return _BaseConnection;
			}
		}
		public DebugDbProviderFactory ProviderFactory { get; }


		public override string ConnectionString { get => BaseConnection.ConnectionString; set => BaseConnection.ConnectionString = value; }

		public override string Database => BaseConnection.Database;

		public override string DataSource => BaseConnection.DataSource;

		public override string ServerVersion => BaseConnection.ServerVersion;

		public override ConnectionState State => BaseConnection.State;

		public DebugDbConnection(DbConnection conn, DebugDbProviderFactory DbProviderFactory)
		{
			this.ProviderFactory = DbProviderFactory;
			_BaseConnection = conn;
			conn.StateChange += Conn_StateChange;
		}
		public override DataTable GetSchema()
		{
			return BaseConnection.GetSchema();
		}
		public override DataTable GetSchema(string collectionName)
		{
			return BaseConnection.GetSchema(collectionName);
		}
		public override DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			return BaseConnection.GetSchema(collectionName, restrictionValues);
		}


		private void Conn_StateChange(object sender, StateChangeEventArgs e)
		{
			OnStateChange(new StateChangeEventArgs(e.OriginalState, e.CurrentState));
		}

		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			return new DebugDbTransaction(BaseConnection.BeginTransaction(isolationLevel), this);
		}

		public override void Close()
		{
			BaseConnection.Close();
		}

		public override void ChangeDatabase(string databaseName)
		{
			BaseConnection.ChangeDatabase(databaseName);
		}

		protected override DbCommand CreateDbCommand()
		{
			return new DebugDbCommand(BaseConnection.CreateCommand(), this);
		}

		public override void Open()
		{
			BaseConnection.Open();
		}
		public override int ConnectionTimeout => BaseConnection.ConnectionTimeout;
		protected override void Dispose(bool disposing)
		{
			Disposable.Release(ref _BaseConnection);
			base.Dispose(disposing);
		}
		protected override DbProviderFactory DbProviderFactory => ProviderFactory;

		
	}
	class DebugDbProviderFactory : DbProviderFactory
	{
		public static DbProviderFactory Instance { get; } = new DebugDbProviderFactory(null);

		public DbProviderFactory BaseFactory { get; }

		public DebugDbProviderFactory(DbProviderFactory BaseFactory)
		{
			this.BaseFactory = BaseFactory;
		}
		public override DbCommand CreateCommand()
		{
			return new DebugDbCommand();
		}
		public override DbConnection CreateConnection()
		{
			return new DebugDbConnection(null, this);
		}
		public override DbCommandBuilder CreateCommandBuilder()
		{
			return base.CreateCommandBuilder();
		}
		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			return BaseFactory.CreateConnectionStringBuilder();
		}
		public override DbDataSourceEnumerator CreateDataSourceEnumerator()
		{
			return BaseFactory.CreateDataSourceEnumerator();
		}
		public override DbDataAdapter CreateDataAdapter()
		{
			return BaseFactory.CreateDataAdapter();
		}
		public override DbParameter CreateParameter()
		{
			return BaseFactory.CreateParameter();
		}
		public override bool CanCreateDataSourceEnumerator => BaseFactory.CanCreateDataSourceEnumerator;
		//public override CodeAccessPermission CreatePermission(PermissionState state)
		//{
		//	return BaseFactory.CreatePermission(state);
		//}


	}
}
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
		public DbConnection Connect()
		{
			var connection = Factory.CreateConnection();
			connection.ConnectionString = Config.ConnectionString;
			connection.Disposed += Connection_Disposed;
			return connection;
			//return new System.Data.Debug.DebugDbConnection(connection, new System.Data.Debug.DebugDbProviderFactory(Factory));
		}

		private void Connection_Disposed(object sender, EventArgs e)
		{
			
		}
	}
}
