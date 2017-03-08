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

namespace SF.Core.DI
{
	public static class InterceptionDIServiceCollectionExtension
	{
		public static IDIServiceCollection UseInterceptingProxyTypeBuilder(this IDIServiceCollection sc)
		{
			sc.AddSingleton<IInterceptingProxyTypeBuilder, InterceptingProxyTypeBuilder>();
			return sc;
		}
		public static IDIServiceCollection UseProxyInvokerFactory(this IDIServiceCollection sc)
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
