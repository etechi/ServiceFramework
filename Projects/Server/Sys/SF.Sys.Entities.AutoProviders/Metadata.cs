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
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SF.Sys.Entities.AutoEntityProvider
{
	public interface IAttribute
	{
		string Name { get; }
		IReadOnlyDictionary<string,object> Values { get; }		
	}
	public interface IMetaItem
	{
		string Name { get; }
		IReadOnlyList<IAttribute> Attributes { get; }
	}
	public enum PropertyMode
	{
		Value,
		SingleRelation,
		MultipleRelation
	}
	public interface IProperty : IMetaItem
	{

		IType Type { get; }
		PropertyMode Mode{ get; }
	}
	public interface IType : IMetaItem
	{
		
	}
	//public interface IValueMapper
	//{
	//	Type DataType { get; }
	//	Type TempType { get; }
	//	Type ModelType { get; }

	//	Expression DataValueToTempValue(Expression DataValue);
	//	Expression TempValueToDataValue(Expression TempValue);
	//	Expression TempValueToModelValue(Expression TempValue);
	//	Expression ModelValueToTempValue(Expression ModelValue);
	//}

	//public interface IValueTypeProvider
	//{
	//	IValueMapper DetailValueMapper { get; }
	//	IValueMapper SummaryValueMapper { get; }
	//	IValueMapper EditableValueMapper { get; }
	//}

	public interface IValueType : IType
	{
		//IValueTypeProvider Provider { get; }
		Type SysType { get; }
	}
	
	public interface IEntityType : IType
	{
		IReadOnlyList<IProperty> Properties { get; }
	}
	public interface IMetadataCollection 
	{
		IReadOnlyDictionary<string, IEntityType> EntityTypes { get; }
		IReadOnlyDictionary<Type, IEntityType> EntityTypesByType { get; }
	}
}
