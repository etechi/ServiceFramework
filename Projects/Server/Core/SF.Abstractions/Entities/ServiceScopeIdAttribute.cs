using System;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using System.Threading.Tasks;

namespace SF.Entities
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ServiceScopeIdAttribute:Attribute
	{	
	}
}
