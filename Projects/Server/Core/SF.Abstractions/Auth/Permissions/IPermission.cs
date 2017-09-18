using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth
{
	public interface IPermission
	{
		string OperationId { get;}
		string ResourceId { get; }
	}

	public class Permission : IPermission
	{
		public Permission() { }
		public Permission(string ResourceId,string OperationId) {
			this.ResourceId = ResourceId;
			this.OperationId = OperationId;
		}
		public override string ToString()
		{
			return ResourceId + ":" + OperationId;
		}
		public string OperationId { get; set; }
		public string ResourceId { get; set; }
	}
}
