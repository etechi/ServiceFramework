using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data
{
	[AttributeUsage(AttributeTargets.Property,AllowMultiple =true)]
	public class IndexAttribute : Attribute
	{
		public IndexAttribute() { }
		public IndexAttribute(string name) { this.Name = name; }
		public IndexAttribute(string name, int order)
		{
			this.Name = name;
			this.Order = order;
		}
		public virtual bool IsClustered { get; set; }
		public virtual bool IsUnique { get; set; }
		public virtual string Name { get;  }
		public virtual int Order { get; set; }
	}

}
