using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Metadata
{
	[AttributeUsage(AttributeTargets.Class)]
	public class EntityRelatedAttribute : Attribute
	{
		public string Entity { get; }
		public EntityRelatedAttribute(string Entity)
		{
			this.Entity = Entity;
		}
	}


}
