#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;

using System.Collections.Generic;

namespace SF.Sys.Services
{
	public interface IServiceImplement
	{
		string Name { get; }
		string ImplementName { get; }
		Type ServiceType { get; }
		Type ImplementType { get; }
		object ImplementInstance { get; }
		Func<IServiceProvider, object> ImplementCreator { get; }
		System.Reflection.MethodInfo ImplementMethod { get; }
		ServiceImplementType ServiceImplementType { get; }
		ServiceImplementLifetime LifeTime { get; }
		bool IsManagedService { get; }
		bool IsDataScope { get; }
		IManagedServiceInitializer ManagedServiceInitializer { get; }
	}

	public interface IServiceDeclaration
	{
		bool HasManagedServiceImplement{get;}
		bool HasUnmanagedServiceImplement { get; }
		string ServiceName { get; }
		Type ServiceType { get; }
		IReadOnlyList<IServiceImplement> Implements { get; }
	}
	public interface IServiceMetadata : IServiceDetector
	{
		IReadOnlyDictionary<string, IServiceDeclaration> ServicesById { get; }
		IReadOnlyDictionary<string, IServiceImplement> ImplementsById { get; }
		IReadOnlyDictionary<string, IServiceDeclaration> ServicesByTypeName { get; }
		IReadOnlyDictionary<string, IServiceImplement[]> ImplementsByTypeName { get; }
		IReadOnlyDictionary<Type, IServiceDeclaration> Services { get; }
	}

}