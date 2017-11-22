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

using System.Collections.Generic;
using System.Linq;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.DataModelBuilders.Providers
{


	public class DefaultDataModelBuildProvider : IDataModelBuildProvider
	{
		IDataModelTypeBuildProvider[] DataModelTypeBuildProviders { get; }
		public DefaultDataModelBuildProvider(IEnumerable<IDataModelTypeBuildProvider> DataModelTypeBuildProviders)
		{
			this.DataModelTypeBuildProviders = DataModelTypeBuildProviders.OrderBy(p => p.Priority).ToArray();
		}

		public int Priority => 0;

		public void AfterBuildModel(IDataModelBuildContext Context)
		{
			foreach (var type in Context.TypeExpressions)
				foreach (var provider in DataModelTypeBuildProviders)
					provider.AfterBuildType(Context, type.Value, Context.Metadata.EntityTypes[type.Key]);
		}

		public void BeforeBuildModel(IDataModelBuildContext Context)
		{
			foreach (var type in Context.TypeExpressions)
				foreach (var provider in DataModelTypeBuildProviders.Reverse())
					provider.BeforeBuildType(Context, type.Value, Context.Metadata.EntityTypes[type.Key]);
		}
	}
}
