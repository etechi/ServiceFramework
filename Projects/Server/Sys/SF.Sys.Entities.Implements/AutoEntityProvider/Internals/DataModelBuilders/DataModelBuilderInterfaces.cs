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
using System.Collections.Generic;
using SF.Sys.Reflection;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.DataModelBuilders
{

	public class TypeMapResult
	{
		public Type Type { get; set; }
		public IReadOnlyList<IAttribute> Attributes { get; set; }
	}
	public interface IDataModelTypeMapper
	{
		int Priority { get; }
		TypeMapResult MapType(IEntityType EntityType, IProperty Property, Type Type);
	}
	public interface IDataModelAttributeGenerator
	{
		CustomAttributeExpression Generate(IAttribute Attr);
	}
	public interface IDataModelBuildContext
	{
		Dictionary<string, TypeExpression> TypeExpressions{ get; }
		IMetadataCollection Metadata { get; }
		IEnumerable<IEntityType> EntityTypes { get; }
	}
	public interface IDataModelPropertyBuildProvider
	{
		int Priority { get; }
		PropertyExpression BeforeBuildProperty(
			IDataModelBuildContext Context, 
			TypeExpression Type, 
			PropertyExpression Property, 
			IEntityType EntityType,
			IProperty EntityProperty
			);
		PropertyExpression AfterBuildProperty(
			IDataModelBuildContext Context, 
			TypeExpression Type, 
			PropertyExpression Property, 
			IEntityType EntityType, 
			IProperty EntityProperty
			);
	}
	public interface IDataModelTypeBuildProvider
	{
		int Priority { get; }
		void BeforeBuildType(
			IDataModelBuildContext Context, 
			TypeExpression Type, 
			IEntityType Entity
			);
		void AfterBuildType(
			IDataModelBuildContext Context, 
			TypeExpression Type, 
			IEntityType Entity
			);
	}
	public interface IDataModelBuildProvider
	{
		int Priority { get; }
		void BeforeBuildModel(IDataModelBuildContext Context);
		void AfterBuildModel(IDataModelBuildContext Context);
	}
	

}
