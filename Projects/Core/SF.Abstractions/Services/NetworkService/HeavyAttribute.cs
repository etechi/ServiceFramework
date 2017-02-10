using SF.Metadata.Models;
using System;
namespace SF.Services.NetworkService
{
	[AttributeUsage(AttributeTargets.Method)]
	public class HeavyMethodAttribute : System.Attribute
	{
	}

}
