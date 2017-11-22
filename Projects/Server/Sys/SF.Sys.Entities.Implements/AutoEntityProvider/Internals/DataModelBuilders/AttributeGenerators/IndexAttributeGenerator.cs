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
using System.Collections.Generic;
using SF.Sys.Reflection;
using SF.Sys.Collections.Generic;
using SF.Sys.Data;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.DataModelBuilders.AttributeGenerators
{
	public class IndexAttributeGenerator : IDataModelAttributeGenerator
	{
		public CustomAttributeExpression Generate(IAttribute Attr)
		{
			var props = new List<PropertyInfo>();
			var args = new List<object>();
			var IsClustered = Attr.Values?.Get("IsClustered");
			var IsUnique = Attr.Values?.Get("IsUnique");
			var Name = Attr.Values?.Get("Name");
			var Order = Attr.Values?.Get("Order");
			return new CustomAttributeExpression(
				typeof(IndexAttribute).GetConstructor(Array.Empty<Type>()),
				Array.Empty<object>(),
				new[]
				{
					typeof(IndexAttribute).GetProperty("IsClustered"),
					typeof(IndexAttribute).GetProperty("IsUnique"),
					typeof(IndexAttribute).GetProperty("Name"),
					typeof(IndexAttribute).GetProperty("Order"),
				},
				new object[]
				{
					Convert.ToBoolean(Attr.Values?.Get("IsClustered")??null),
					Convert.ToBoolean(Attr.Values?.Get("IsUnique")??null),
					Convert.ToString(Attr.Values?.Get("Name")??null),
					Convert.ToInt32(Attr.Values?.Get("Order")??"0")
				}
			);
		}

		CustomAttributeExpression IDataModelAttributeGenerator.Generate(IAttribute Attr)
		{
			throw new NotImplementedException();
		}
	}

}
