using System.Collections.Generic;

namespace SF.Clients
{
	public interface IIdentity
	{
		Claim[] Claims { get; }
	}
}
