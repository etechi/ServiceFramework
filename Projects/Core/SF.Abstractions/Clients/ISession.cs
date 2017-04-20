using System.Collections.Generic;

namespace SF.Clients
{
	public interface ISession
	{
		string Id { get; }
		IIdentity User { get; }
	}
}
