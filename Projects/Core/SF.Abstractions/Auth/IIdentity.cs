using System.Collections.Generic;

namespace SF.Auth
{
	public interface IIdentity
	{
		Claim[] Claims { get; }
	}
}
