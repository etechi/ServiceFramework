using SF.Core.ServiceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SF.Data
{
	public interface IEntityProperty
	{
		string Name { get; }
		PropertyInfo PropertyInfo { get; }
	}
	public interface IDataSetMetadata
	{
		string EntitySetName { get; }
		IEntityProperty[] Key { get; }
		IEntityProperty[] Properties { get; }
	}
}
