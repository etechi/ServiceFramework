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
using SF.Sys.Reflection;
using SF.Sys.Linq;
namespace SF.Sys.Entities.AutoEntityProvider.Internals.DataModelBuilders.Providers
{


	public class DefaultDataModelTypeBuildProvider : IDataModelTypeBuildProvider
	{
		IDataModelPropertyBuildProvider[] PropertyBuildProviders { get; }
		IDataModelPropertyBuildProvider[] InversePropertyBuildProviders { get; }
		public DefaultDataModelTypeBuildProvider(IEnumerable<IDataModelPropertyBuildProvider> DataModelPropertyBuildProviders)
		{
			PropertyBuildProviders = DataModelPropertyBuildProviders.OrderBy(p => p.Priority).ToArray();
			InversePropertyBuildProviders = this.PropertyBuildProviders.Reverse().ToArray();
		}

		public int Priority => 0;

		public void BeforeBuildType(IDataModelBuildContext Context, TypeExpression Type, IEntityType Entity)
		{
			foreach (var prop in Entity.Properties)
				foreach (var provider in PropertyBuildProviders)
				{
					var idx = Type.Properties.IndexOf(tp => tp.Name == prop.Name);
					var op = idx == -1 ? null : Type.Properties[idx];
					var np= provider.BeforeBuildProperty(Context, Type, op, Entity, prop);
					if (np == op)
						continue;
					if (idx == -1)
					{
						if (np != null)
							Type.Properties.Add(np);
					}
					else
						Type.Properties[idx] = np;
				}
		}

		public void AfterBuildType(IDataModelBuildContext Context, TypeExpression Type, IEntityType Entity)
		{
			foreach (var prop in Entity.Properties)
				foreach (var provider in InversePropertyBuildProviders)
				{
					var idx = Type.Properties.IndexOf(tp => tp.Name == prop.Name);
					var op = idx == -1 ? null : Type.Properties[idx];
					var np = provider.AfterBuildProperty(Context, Type, op, Entity, prop);
					if (np == op)
						continue;
					if (idx == -1)
					{
						if (np != null)
							Type.Properties.Add(np);
					}
					else
						Type.Properties[idx] = np;
				}
		}
	}
}
