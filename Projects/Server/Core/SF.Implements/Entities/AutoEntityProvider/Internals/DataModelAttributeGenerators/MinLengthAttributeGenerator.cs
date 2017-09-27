using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations;

namespace SF.Entities.AutoEntityProvider.Internals.DataModelAttributeGenerators
{
	public class MinLengthAttributeGenerator : IDataModelAttributeGenerator
	{
		public SystemAttributeBuilder Generate(IAttribute Attr)
		{
			var Length = Attr.Values.Get("Length");
			if (Length == null)
				return null;
			else
				return new SystemAttributeBuilder(
					typeof(MinLengthAttribute).GetConstructor(new[] { typeof(int) }),
					new object[] { Convert.ToInt32(Length) }
				);
		}
	}

}
