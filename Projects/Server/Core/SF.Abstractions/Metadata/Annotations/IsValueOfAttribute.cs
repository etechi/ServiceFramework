using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Metadata
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class IsValueOfAttribute : Attribute
	{
		public Type Type { get; }
		public Type ValueType { get; }
		public IsValueOfAttribute(Type Type)
		{
			this.Type = Type;
			this.ValueType = Type.GetProperty("Value").PropertyType;
		}

	}
}
