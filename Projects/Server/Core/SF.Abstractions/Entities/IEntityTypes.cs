using System;
using System.Collections.Generic;

namespace SF.Entities
{
	public interface IEntityServiceDescriptor
	{
		string Ident { get; }
		string Name { get; }
		Type ServiceType { get; }
		IEnumerable<Type> EntityTypes { get; }
	}
	public interface IEntityServiceDescriptorGroup : IEnumerable<IEntityServiceDescriptor>
	{
		string Ident { get; }
		string Name { get; }
	}
}
