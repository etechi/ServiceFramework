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

using System.Linq;
using SF.Sys.Reflection;
using SF.Sys.Annotations;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.DataModelBuilders.Providers
{
	public class DataModelEntityIdentPropertyBuildProvider : IDataModelPropertyBuildProvider
	{
		public int Priority => 100;

		public PropertyExpression AfterBuildProperty(IDataModelBuildContext Context, TypeExpression Type, PropertyExpression Property, IEntityType EntityType, IProperty EntityProperty)
		{
			if(EntityProperty.Attributes?.Any(a=>a.Name==typeof(EntityIdentAttribute).FullName) ?? false)
			{
				DataModelBuildHelper.EnsureSingleFieldIndex(
					Type,
					Property,
					EntityType,
					EntityProperty
					);
			}
			return Property;
		}

		public PropertyExpression BeforeBuildProperty(IDataModelBuildContext Context, TypeExpression Type, PropertyExpression Property, IEntityType EntityType, IProperty EntityProperty)
		{
			return Property;
		}
	}
}
