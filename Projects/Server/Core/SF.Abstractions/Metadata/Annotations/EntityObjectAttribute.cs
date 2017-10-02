using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Metadata
{
	[AttributeUsage(AttributeTargets.Class)]
	public class EntityObjectAttribute : Attribute
	{
		//ID作为对象属于同一实体的标记
		public string Entity { get; }
		public EntityObjectAttribute(string Entity=null)
		{
			this.Entity = Entity;
		}
	}

}
