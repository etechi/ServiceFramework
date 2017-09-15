using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Metadata
{
	[AttributeUsage(AttributeTargets.Property)]
	public class MediaAttribute : Attribute
	{
		public string[] mimes { get; }
		public MediaAttribute(string[] mimes = null)
		{
			this.mimes = mimes;
		}
	}

}
