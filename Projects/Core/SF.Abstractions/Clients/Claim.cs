using System.Collections.Generic;

namespace SF.Clients
{
	
	public class Claim
	{
		public string Issuer { get; set; }
		public string OriginalIssuer { get; set; }
		public KeyValuePair<string, string>[] Properties { get; }
		public string Type { get; set; }
		public string Value { get; set; }
		public string ValueType { get; set; }
	}
}
