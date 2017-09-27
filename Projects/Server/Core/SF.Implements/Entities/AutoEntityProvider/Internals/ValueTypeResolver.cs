using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using SF.Metadata;

namespace SF.Entities.AutoEntityProvider.Internals
{

	public class ValueTypeResolver : IValueTypeResolver
	{
		public IValueType Resolve(string PropName, Type SystemType, IReadOnlyList<IAttribute> Attributes)
		{
			throw new NotImplementedException();
		}
	}
}
