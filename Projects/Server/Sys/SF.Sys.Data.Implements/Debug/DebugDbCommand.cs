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
using System.Threading.Tasks;
using System.Threading;
using SF.Sys;

namespace System.Data.Debug
{
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
}
