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

using SF.Sys.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SF.Sys.Services
{


	public enum ServiceResolveType
	{
		None,
		Direct,
		Lazy,
		Func,
		ResolveByIdent,
		Enumerable
	}
	public static class ServiceDetectorExtension
	{
		public static ServiceResolveType DetectService(this IServiceDetector Detector, Type Type, out Type RealType)
		{
			ServiceResolveType ResolveType = ServiceResolveType.Direct;
			RealType = Type;

			if (Type.IsGeneric())
			{
				var gtd = Type.GetGenericTypeDefinition();
				if (gtd == typeof(Lazy<>))
					ResolveType = ServiceResolveType.Lazy;
				else if (gtd == typeof(Func<>))
					ResolveType = ServiceResolveType.Func;
				else if (gtd == typeof(TypedInstanceResolver<>))
					ResolveType = ServiceResolveType.ResolveByIdent;
				else if (gtd == typeof(IEnumerable<>))
					return ServiceResolveType.Enumerable;

				if (ResolveType != ServiceResolveType.Direct)
					RealType = Type.GetGenericArguments()[0];
			}
			if (Detector.IsService(RealType))
				return ResolveType;
			if (RealType.IsGeneric() && Detector.IsService(RealType.GetGenericTypeDefinition()))
				return ResolveType;
			if (ResolveType != ServiceResolveType.Direct)
				throw new InvalidOperationException($"未定义服务:{RealType}");
			return ServiceResolveType.None;
		}
		public static ServiceResolveType DetectService(this IServiceDetector Detector, Type Type)
		{
			return Detector.DetectService(Type, out var rt);
		}
	}
}
