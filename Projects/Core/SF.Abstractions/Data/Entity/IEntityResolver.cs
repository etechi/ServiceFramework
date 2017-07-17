using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SF.Core.ServiceManagement;

namespace SF.Data.Entity
{
	public interface IDataEntity
	{
		string Ident { get; }
		string Name { get; }
	}
	[UnmanagedService]
	public interface IDataEntityResolver
	{
        Task<IDataEntity[]> Resolve(string Type,string[] Keys);
    }
}
