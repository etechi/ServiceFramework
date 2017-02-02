using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Auth
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
	public class RequirePermissionAttribute : Attribute
	{
		public string Resource { get; set; }
		public string Operation { get; set; }
		public RequirePermissionAttribute(string Operation)
		{
			this.Operation = Operation;
		}
	}
}
