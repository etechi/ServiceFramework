using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SF.Entities.AutoEntityProvider
{
	[Flags]
	public enum AutoCapability
	{
		GenerateManager=1,
		GenerateDataModel=2
	}
	public interface IEntityAutoCapability
	{
		string EntityIdent { get; }
		AutoCapability Capability { get; }
	}
}
