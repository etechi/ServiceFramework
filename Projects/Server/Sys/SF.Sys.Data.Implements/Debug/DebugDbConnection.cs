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

using System.Data.Common;
using SF.Sys;

namespace System.Data.Debug
{
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
}
