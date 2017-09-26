using System;
using System.Collections.Generic;

namespace SF.Entities.AutoEntityProvider
{
	public interface IValueTypeResolver
	{
		IValueType Resolve(string PropName, Type SystemType, IReadOnlyList<IAttribute> Attributes);
	}
}
