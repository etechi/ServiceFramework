using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Metadata
{
	[AttributeUsage(AttributeTargets.Class)]
	public class EntityObjectAttribute : Attribute
	{
		public string Id { get; }
		public EntityObjectAttribute(string Id=null)
		{
			this.Id = Id;
		}
	}

}
