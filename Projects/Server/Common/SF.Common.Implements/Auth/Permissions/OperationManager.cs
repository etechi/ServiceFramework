using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Permissions.Models;
using SF.Entities;
namespace SF.Auth.Permissions
{
	public class OperationManager :
		ConstantObjectQueryableEntitySource<string, Models.OperationInternal>,
		IOperationManager
	{
		public OperationManager(IEntityManager EntityManager, IReadOnlyDictionary<string, OperationInternal> Models) : base(EntityManager, Models)
		{
		}
	}
}
