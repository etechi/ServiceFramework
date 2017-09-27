using System;

namespace SF.Entities.AutoEntityProvider.Internals
{
	public class AutoEntityType
	{
		public AutoEntityType(string Namespace,Type Type)
		{
			this.Namespace = Namespace;
			this.Type = Type;
		}
		public string Namespace { get; }
		public Type Type { get;  }
	}
}
