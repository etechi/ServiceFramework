using System.Collections.Generic;

namespace SF.Clients
{
	public enum ClientDeviceType
	{
		PCDesktop,
		PCBrowser,
		WinXin,
		Andriod,
		iPhone,
		WAP,
		Console
	}
	public interface IClientDeviceTypeDetector{
		ClientDeviceType Detect(string agent);
	}
}
