using SF.Metadata;
using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace SF.Core.Hosting
{
	public class DynamicModuleBuilder : IDynamicModuleBuilder
	{
		 //AssemblyBuilder AssemblyBuilder { get; } =
			//   AssemblyBuilder.DefineDynamicAssembly(
			//	   new AssemblyName("SFAutoEntityProviderDynamicClass"),
			//	   AssemblyBuilderAccess.Run
			//	   );
		 ModuleBuilder ModuleBuilder { get; } //= AssemblyBuilder.DefineDynamicModule(new Guid().ToString("N"));

		public object SystemModuleBuilder => ModuleBuilder;
	}

}
