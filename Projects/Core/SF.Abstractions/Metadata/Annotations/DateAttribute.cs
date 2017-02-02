using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Metadata
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DateAttribute : Attribute
	{
		public bool EndTime { get; set; }
		public DateAttribute()
		{
		}
	}

}
