using System;

using System.Collections.Generic;

namespace SF.Core.ServiceManagement
{
	public interface IServiceImplement
	{
		Type ServiceType { get; }
		Type ImplementType { get; }
		object ImplementInstance { get; }
		Func<IServiceProvider, object> ImplementCreator { get; }

		ServiceImplementType ServiceImplementType { get; }
		ServiceImplementLifetime LifeTime { get; }
		bool IsManagedService { get; }
	}
	public interface IServiceDeclaration
	{
		Type ServiceType { get; }
		IReadOnlyList<IServiceImplement> Implements { get; }
	}
	public interface IServiceMetadata : IServiceDetector
	{
		IReadOnlyDictionary<string, IServiceDeclaration> ServicesByTypeName { get; }
		IReadOnlyDictionary<Type, IServiceDeclaration> Services { get; }
	}

}
