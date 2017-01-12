using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data.Entity
{
	public interface IDataContextProviderFactory
	{
		IDataContextProvider Create(string Name);
	}

}
