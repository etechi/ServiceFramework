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
using System.Reflection;
using System.Threading.Tasks;
using SF.Core.Serialization;
using SF.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SF.Entities.AutoEntityProvider.Internals.PropertyQueryConveters
{
	public class EntityNameQueryConverterProvider : IEntityPropertyQueryConverterProvider
	{
		public int Priority => 0;
			
		class MultipleRelationConverter : IEntityPropertyQueryConverter
		{
			public Type TempFieldType => null;
			public PropertyInfo SingleRelationProp { get; }
			public PropertyInfo NameProp { get; }
			public MultipleRelationConverter(PropertyInfo SingleRelationProp,PropertyInfo NameProp)
			{
				this.SingleRelationProp = SingleRelationProp;
				this.NameProp = NameProp;
			}
			public Expression SourceToDestOrTemp(Expression src, int level, PropertyInfo srcProp, PropertyInfo dstProp)
			{
				return src.GetMember(SingleRelationProp).GetMember(NameProp);
			}
		}
		
		public IEntityPropertyQueryConverter GetPropertyConverter(Type DataModelType, PropertyInfo DataModelProperty, Type EntityType, PropertyInfo EntityProperty,QueryMode QueryMode)
		{
			if (QueryMode == QueryMode.Edit)
				return null;
			if (DataModelProperty != null)
				return null;
			if (EntityProperty.PropertyType != typeof(string))
				return null;

			var fkField=EntityProperty.ReflectedType.AllPublicInstanceProperties().FirstOrDefault(p =>
			{
				var a = p.GetCustomAttribute<EntityIdentAttribute>();
				return a != null && a.NameField == EntityProperty.Name;
			});
			if (fkField == null)
				return null;

			var modelFkField = DataModelType.GetProperty(fkField.Name);
			if (modelFkField == null)
				return null;

			var modelSingleRelationProp = DataModelType.AllPublicInstanceProperties().FirstOrDefault(p =>
			  {
				  var fk=p.GetCustomAttribute<ForeignKeyAttribute>();
				  return fk != null && fk.Name == fkField.Name;
			  });

			if (modelSingleRelationProp == null)
				return null;

			var nameProp = modelSingleRelationProp.PropertyType.GetProperty("Name");
			if (nameProp == null)
				return null;

			return new MultipleRelationConverter(modelSingleRelationProp, nameProp);
		}
	}

}
