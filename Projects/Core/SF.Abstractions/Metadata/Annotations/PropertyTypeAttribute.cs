using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Metadata
{
	public enum PropertyTypeSourceType
	{
		Internal,
		External
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class PropertyTypeAttribute : Attribute
	{
		public PropertyTypeSourceType TypeSourceType { get; }
		public string TypeSourceField { get; }
		public PropertyTypeAttribute(PropertyTypeSourceType TypeSourceType, string TypeSourceField)
		{
			this.TypeSourceType = TypeSourceType;
			this.TypeSourceField = TypeSourceField;
		}
	}

}
