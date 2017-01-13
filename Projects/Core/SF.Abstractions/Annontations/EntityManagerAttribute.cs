using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Annotations
{
	[AttributeUsage(AttributeTargets.Class)]
	public class EntityManagerAttribute : Attribute
	{
		public string Entity { get; }
		public string Title { get; set; }
		public string IconClass { get; set; }
		public EntityManagerAttribute(string Entity)
		{
			this.Entity = Entity;
		}
	}


}
