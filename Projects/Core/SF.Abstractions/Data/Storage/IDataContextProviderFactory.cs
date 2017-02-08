using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data.Storage
{
	public interface IDataContextProviderFactory
	{
		IDataContextProvider Create(IDataContext Context);
	}


}
