using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace SF.Data.Entity
{
	public interface IDataEntity
	{
		string Ident { get; }
		string Name { get; }
	}
	public interface IDataEntityResolver
	{
        Task<IDataEntity[]> Resolve(string Type,string[] Keys);
    }
}
