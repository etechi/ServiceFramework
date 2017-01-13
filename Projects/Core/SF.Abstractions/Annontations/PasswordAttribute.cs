using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Annotations
{
	[AttributeUsage(AttributeTargets.Property)]
	public class PasswordAttribute : MediaAttribute
	{
		public PasswordAttribute()
		{

		}
	}
}
