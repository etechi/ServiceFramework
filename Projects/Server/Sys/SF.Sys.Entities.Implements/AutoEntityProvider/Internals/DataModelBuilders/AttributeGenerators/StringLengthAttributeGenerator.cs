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
using System.ComponentModel.DataAnnotations;
using SF.Sys.Reflection;
using SF.Sys.Collections.Generic;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.DataModelBuilders.AttributeGenerators
{
	public class StringLengthAttributeGenerator : IDataModelAttributeGenerator
	{
		public CustomAttributeExpression Generate(IAttribute Attr)
		{
			var MaximumLength = Attr.Values.Get("MaximumLength");
			var min = Convert.ToInt32(Attr.Values.Get("MinimumLength"));
			if (MaximumLength == null)
			{
				if (min == 0)
					return null;
				return new CustomAttributeExpression(
					typeof(StringLengthAttribute).GetConstructor(Array.Empty<Type>()),
					Array.Empty<object>(),
					new PropertyInfo[] { typeof(StringLengthAttribute).GetProperty("MinimumLength", BindingFlags.Public | BindingFlags.Instance) },
					new object[] { min }
					);
			}
			else if (min == 0)
				return new CustomAttributeExpression(
					typeof(StringLengthAttribute).GetConstructor(new[] { typeof(int) }),
					new object[] { Convert.ToInt32(MaximumLength) }
				);
			else
				return new CustomAttributeExpression(
					typeof(StringLengthAttribute).GetConstructor(new[] { typeof(int) }),
					new object[] { Convert.ToInt32(MaximumLength) },
					new PropertyInfo[] { typeof(StringLengthAttribute).GetProperty("MinimumLength", BindingFlags.Public | BindingFlags.Instance) },
					new object[] { min }
				);
		}
	}

}
