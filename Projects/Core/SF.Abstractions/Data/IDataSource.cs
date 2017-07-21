using SF.Core.ServiceManagement;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data
{ 
	[UnmanagedService]
	public interface IDataSource
	{
		DbConnection Connect();
	}
}
