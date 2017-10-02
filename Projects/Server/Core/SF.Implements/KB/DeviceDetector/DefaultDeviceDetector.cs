using SF.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.KB.DeviceDetector.Providers
{
	public class DefaultDeviceTypeDetector : IClientDeviceTypeDetector
	{
		public ClientDeviceType Detect(string agent)
		{
			return ClientDeviceType.PCBrowser;
		}
	}
}
