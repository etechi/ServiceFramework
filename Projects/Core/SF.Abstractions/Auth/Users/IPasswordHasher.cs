using SF.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
	public interface IPasswordHasher
	{
		string Hash(string Value);
	}
}

