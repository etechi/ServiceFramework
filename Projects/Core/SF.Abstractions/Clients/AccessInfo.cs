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
		WAP
	}
	public class Claim
	{
		public string Issuer { get; set; }
		public string OriginalIssuer { get; set; }
		public KeyValuePair<string, string>[] Properties { get; }
		public string Type { get; set; }
		public string Value { get; set; }
		public string ValueType { get; set; }
	}
	public class Identity
	{
		public Claim[] Claims { get; set; }
	}
	public class AccessInfo
	{
		public Identity Identity { get; set; }
		public string ClientAddress { get; set; }
		public string ClientAgent { get; set; }
		public ClientDeviceType DeviceType { get; set; }
	}
}
