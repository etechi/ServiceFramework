using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Permissions.Models;
using SF.Entities;
namespace SF.Auth.Permissions
{
	public class OperationDictionary:
		Dictionary<string, OperationInternal>,
		IReadOnlyDictionary<string, OperationInternal>
	{
		public OperationDictionary(IEnumerable<IOperation> items)
		{
			foreach (var it in items)
				Add(it.Id, new OperationInternal
				{
					Id=it.Id,
					CreatedTime=DateTime.Now,
					LogicState=EntityLogicState.Enabled,
					Name=it.Name,
					UpdatedTime=DateTime.Now,
					Description=it.Description
				});
		}
	}
}
