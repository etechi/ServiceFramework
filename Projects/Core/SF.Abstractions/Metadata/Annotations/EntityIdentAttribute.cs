using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Metadata
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = true)]
	public class EntityIdentAttribute : Attribute
	{
		public string Entity { get; }
		public string NameField { get; }
		public int Column { get; }
		public string ScopeField { get; set; }
		public object ScopeValue { get; set; }
		public bool IsTreeParentId { get; set; }
		public string MultipleKeyField { get; set; }
		public EntityIdentAttribute(string Entity = null, string NameField = null, int Column = 0, string MultipleKeyField = null)
		{
			this.Entity = Entity;
			this.NameField = NameField;
			this.Column = Column;
			this.MultipleKeyField = MultipleKeyField;
		}
	}

}
