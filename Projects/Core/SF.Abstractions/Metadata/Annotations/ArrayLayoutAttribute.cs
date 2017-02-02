using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Metadata
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ArrayLayoutAttribute : Attribute
	{
		public bool HertMode { get; set; }
		public ArrayLayoutAttribute(bool HertMode = false)
		{
			this.HertMode = HertMode;
		}
	}


}
