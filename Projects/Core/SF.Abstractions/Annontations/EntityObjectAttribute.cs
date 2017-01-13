using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Annotations
{
	[AttributeUsage(AttributeTargets.Class)]
	public class EntityObjectAttribute : Attribute
	{
		public string Entity { get; }
		public EntityObjectAttribute(string Entity)
		{
			this.Entity = Entity;
		}
	}

}
