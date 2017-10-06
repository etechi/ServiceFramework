using SF.Core.ServiceManagement;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SF.Entities
{
	[AttributeUsage(AttributeTargets.Interface)]
	public class AutoTestAttribute : Attribute
	{

	}
}
