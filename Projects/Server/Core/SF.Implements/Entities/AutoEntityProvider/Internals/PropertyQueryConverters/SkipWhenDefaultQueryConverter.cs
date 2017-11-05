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

namespace SF.Entities.AutoEntityProvider.Internals.PropertyQueryConveters
{
	class NoneQueryConverter : IEntityPropertyQueryConverter
	{
		NoneQueryConverter() { }
		public static NoneQueryConverter Instance { get; } = new NoneQueryConverter();
		public Type TempFieldType => null;

		public Expression SourceToDestOrTemp(Expression src, int level, PropertyInfo srcProp, PropertyInfo dstProp)
		{
			return null;
		}
	}
	public class SkipWhenDefaultQueryConverterProvider : IEntityPropertyQueryConverterProvider
	{
		public int Priority => -10000;

		public IEntityPropertyQueryConverter GetPropertyConverter(Type DataModelType, PropertyInfo DataModelProperty, Type EntityType, PropertyInfo EntityProperty, QueryMode QueryMode)
		{
			if (DataModelProperty == null ||
				EntityProperty == null			
				)
				return null;
			if (EntityProperty.GetCustomAttribute<SkipWhenDefaultAttribute>() != null)
				return NoneQueryConverter.Instance;
			return null;
				
		}
	}

}
