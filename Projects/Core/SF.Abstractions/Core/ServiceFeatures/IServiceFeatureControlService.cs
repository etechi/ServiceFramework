using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.ServiceFeatures
{	
	[NetworkService]
	[Auth.Authorize("sysadmin")]
	public interface IServiceFeatureControlService
	{
		Task<string> Init();
	}
}
