using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Metadata
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
	public class HiddenAttribute : Attribute
	{
	}
}
