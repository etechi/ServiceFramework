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
using System.Reflection;
using System.Linq;
using SF.Sys.Reflection;
using SF.Sys.Entities.Annotations;
using SF.Sys.Annotations;
using SF.Sys.Data;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.DataModelBuilders.Providers
{

	public class DataModelServiceScopeProvider : IDataModelTypeBuildProvider
	{
		public int Priority => 100;
		public static string RelatedServiceIdName { get; } = "_RelatedServiceId";

		public void AfterBuildType(IDataModelBuildContext Context, TypeExpression Type, IEntityType Entity)
		{
			if (!Entity.Attributes?.Any(a => a.Name == typeof(ServiceScopeEnabledAttribute).FullName)??false)
				return;

			var prop = Type.Properties.FirstOrDefault(p => p.Name == RelatedServiceIdName);
			if (prop != null)
				throw new InvalidOperationException($"创建服务分区字段时错误，对象中已定义名为{RelatedServiceIdName}");

			prop= new PropertyExpression(
				RelatedServiceIdName,
				new SystemTypeReference(typeof(long?)),
				PropertyAttributes.None
				);

			prop.CustomAttributes.Add(
				new CustomAttributeExpression(
					typeof(IndexAttribute).GetConstructor(Array.Empty<Type>())
					)
				);

			prop.CustomAttributes.Add(
				new CustomAttributeExpression(
					typeof(ServiceScopeIdAttribute).GetConstructor(Array.Empty<Type>())
					)
				);

			Type.Properties.Add(prop);
		}

		public void BeforeBuildType(IDataModelBuildContext Context, TypeExpression Type, IEntityType Entity)
		{

		}
	}
}
