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

namespace System.Data.Debug
{
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
