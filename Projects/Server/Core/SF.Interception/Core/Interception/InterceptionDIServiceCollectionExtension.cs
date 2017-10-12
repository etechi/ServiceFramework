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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;
using SF.Core.Interception;

namespace SF.Core.ServiceManagement
{
	public static class InterceptionDIServiceCollectionExtension
	{
		public static IServiceCollection UseInterceptingProxyTypeBuilder(this IServiceCollection sc)
		{
			sc.AddSingleton<IInterceptingProxyTypeBuilder, InterceptingProxyTypeBuilder>();
			return sc;
		}
		public static IServiceCollection UseProxyInvokerFactory(this IServiceCollection sc)
		{
			sc.AddSingleton<IProxyInvokerFactory, ProxyInvokerFactory>();
			return sc;
		}
	}


	//public delegate object DynamicInterfaceProvider(string Member, object[] Args);
	//public delegate object DynamicInterfaceFactory(DynamicInterfaceProvider Provider);


	//public class DynamicInterfaceFactoryBuilder
	//{
	//	public static ModuleBuilder ModuleBuilder { get; }
	//	static DynamicInterfaceFactoryBuilder()
	//	{
	//		var asmName = new AssemblyName("DynamicInterfaceImplement");
	//		var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
	//			asmName, AssemblyBuilderAccess.Run);
	//		ModuleBuilder = asmBuilder.DefineDynamicModule("DynamicInterfaceImplement", "DynamicInterfaceImplement.dll");
	//	}
	//	static void BuildInterface(TypeBuilder typeBuilder, Type InterfaceType)
	//	{
	//		typeBuilder.AddInterfaceImplementation()

	//	}

	//	public static DynamicInterfaceFactory Build(Type InterfaceType)
	//	{
	//		if (!InterfaceType.IsInterfaceType())
	//			throw new NotSupportedException();
	//		var typeBuilder = ModuleBuilder.DefineType(InterfaceType.FullName + "Proxy");
	//		BuildInterface(typeBuilder, InterfaceType);
	//		var implType=typeBuilder.CreateType();
	//		var arg = Expression.Parameter(typeof(DynamicInterfaceProvider),"arg");
	//		return Expression.Lambda<DynamicInterfaceFactory>(
	//				Expression.New(
	//					implType.GetConstructor(new[] { typeof(DynamicInterfaceProvider) }),
	//					arg
	//					),
	//				arg
	//				).Compile();
	//	}
	//	public static DynamicInterfaceFactory Build<I>()
	//	{
	//		return Build(typeof(I));
	//	}
	//}
}
