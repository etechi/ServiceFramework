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
namespace SF.Core.Interception
{
	internal static class IInterceptingProxyMethods
	{
		internal static MethodInfo AddInterceptionBehavior
		{
			get { return StaticReflection.GetMethodInfo<IInterceptingProxy>(ip => ip.AddInterceptionBehavior(null)); }
		}
	}
	
	/// <summary>
	 /// This class provides the code needed to implement the <see cref="IInterceptingProxy"/>
	 /// interface on a class.
	 /// </summary>
	internal static class InterceptingProxyImplementor
	{
		internal static FieldBuilder ImplementIInterceptingProxy(TypeBuilder typeBuilder)
		{
			typeBuilder.AddInterfaceImplementation(typeof(IInterceptingProxy));
			FieldBuilder proxyInterceptorPipelineField =
				typeBuilder.DefineField(
					"pipeline",
					typeof(InterceptionBehaviorPipeline),
					FieldAttributes.Private | FieldAttributes.InitOnly);

			ImplementAddInterceptionBehavior(typeBuilder, proxyInterceptorPipelineField);

			return proxyInterceptorPipelineField;
		}

		private static void ImplementAddInterceptionBehavior(TypeBuilder typeBuilder, FieldInfo proxyInterceptorPipelineField)
		{
			// Declaring method builder
			// Method attributes
			const MethodAttributes MethodAttributes = default(MethodAttributes)
				| (MethodAttributes)(
				1 //| MethodAttributes.Private 
				| 64 //MethodAttributes.Virtual
				| 32 //MethodAttributes.Final 
				| 128//MethodAttributes.HideBySig 
				| 256) //MethodAttributes.NewSlot
				;

			MethodBuilder methodBuilder =
				typeBuilder.DefineMethod(
					"Microsoft.Practices.Unity.InterceptionExtension.IInterceptingProxy.AddInterceptionBehavior",
					MethodAttributes);

			// Setting return type
			methodBuilder.SetReturnType(typeof(void));
			// Adding parameters
			methodBuilder.SetParameters(typeof(IInterceptionBehavior));
			// Parameter method
			methodBuilder.DefineParameter(1, ParameterAttributes.None, "interceptor");

			ILGenerator il = methodBuilder.GetILGenerator();
			// Writing body
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, proxyInterceptorPipelineField);
			il.Emit(OpCodes.Ldarg_1);
			il.EmitCall(OpCodes.Callvirt, InterceptionBehaviorPipelineMethods.Add, null);
			il.Emit(OpCodes.Ret);
			typeBuilder.DefineMethodOverride(methodBuilder, IInterceptingProxyMethods.AddInterceptionBehavior);
		}
	}
	public partial class InterceptingClassGenerator
	{
		private static ModuleBuilder GetModuleBuilder()
		{
			string moduleName = Guid.NewGuid().ToString("N");
#if DEBUG_SAVE_GENERATED_ASSEMBLY
            return AssemblyBuilder.DefineDynamicModule(moduleName, moduleName + ".dll", true);
#else
			return AssemblyBuilder.DefineDynamicModule(moduleName);
#endif
		}
	}
	/// <summary>
	/// Class that handles generating the dynamic types used for interception.
	/// </summary>
	public partial class InterceptingClassGenerator
	{
		private static readonly AssemblyBuilder AssemblyBuilder;

		private readonly Type typeToIntercept;
		private readonly IEnumerable<Type> additionalInterfaces;
		private Type targetType;
		private GenericParameterMapper mainTypeMapper;

		private FieldBuilder proxyInterceptionPipelineField;
		private TypeBuilder typeBuilder;

		static InterceptingClassGenerator()
		{

#if DEBUG_SAVE_GENERATED_ASSEMBLY
            var aba =AssemblyBuilderAccess.RunAndSave;
#else
			var aba = AssemblyBuilderAccess.Run;
#endif
#if NETSTANDARD2_0
			AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Unity_ILEmit_DynamicClasses"),aba);

#else
			AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
				new AssemblyName("Unity_ILEmit_DynamicClasses"),aba);

#endif
		}

		/// <summary>
		/// Create a new <see cref="InterceptingClassGenerator"/> that will generate a
		/// wrapper class for the requested <paramref name="typeToIntercept"/>.
		/// </summary>
		/// <param name="typeToIntercept">Type to generate the wrapper for.</param>
		/// <param name="additionalInterfaces">Additional interfaces the proxy must implement.</param>
		public InterceptingClassGenerator(Type typeToIntercept, params Type[] additionalInterfaces)
		{
			this.typeToIntercept = typeToIntercept;
			this.additionalInterfaces = additionalInterfaces;
			CreateTypeBuilder();
		}

		/// <summary>
		/// Create the wrapper class for the given type.
		/// </summary>
		/// <returns>Wrapper type.</returns>
		public Type GenerateType()
		{
			AddConstructors();

			int memberCount = 0;
			HashSet<Type> implementedInterfaces = GetImplementedInterfacesSet();
			foreach (var @interface in this.additionalInterfaces)
			{
				memberCount =
					new InterfaceImplementation(this.typeBuilder, @interface, this.proxyInterceptionPipelineField, true)
						.Implement(implementedInterfaces, memberCount);
			}

#if NETSTANDARD2_0
			Type result = typeBuilder.CreateTypeInfo().AsType();

#else
			Type result = typeBuilder.CreateType();
#endif
#if DEBUG_SAVE_GENERATED_ASSEMBLY
            assemblyBuilder.Save("Unity_ILEmit_DynamicClasses.dll");
#endif
			return result;
		}

		

		private void AddConstructors()
		{
			BindingFlags bindingFlags =
				this.typeToIntercept.IsAbstractType()
					? BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic
					: BindingFlags.Public | BindingFlags.Instance;

			foreach (ConstructorInfo ctor in typeToIntercept.GetConstructors(bindingFlags))
			{
				AddConstructor(ctor);
			}
		}

		private void AddConstructor(ConstructorInfo ctor)
		{
			if (!(ctor.IsPublic || ctor.IsFamily || ctor.IsFamilyOrAssembly))
			{
				return;
			}

			MethodAttributes attributes =
				(ctor.Attributes
#if !NETCORE
				& ~MethodAttributes.ReservedMask
#endif
				& ~MethodAttributes.MemberAccessMask)
				| MethodAttributes.Public;

			ParameterInfo[] parameters = ctor.GetParameters();

			Type[] paramTypes = parameters.Select(item => item.ParameterType).ToArray();

			ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(
				attributes, ctor.CallingConvention, paramTypes);

			for (int i = 0; i < parameters.Length; i++)
			{
				ctorBuilder.DefineParameter(i + 1, parameters[i].Attributes, parameters[i].Name);
			}

			ILGenerator il = ctorBuilder.GetILGenerator();

			// Initialize pipeline field
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Newobj, InterceptionBehaviorPipelineMethods.Constructor);
			il.Emit(OpCodes.Stfld, proxyInterceptionPipelineField);

			// call base class constructor
			il.Emit(OpCodes.Ldarg_0);
			for (int i = 0; i < paramTypes.Length; ++i)
			{
				il.Emit(OpCodes.Ldarg, i + 1);
			}

			il.Emit(OpCodes.Call, ctor);

			il.Emit(OpCodes.Ret);
		}

		private void CreateTypeBuilder()
		{
			TypeAttributes newAttributes = typeToIntercept.GetTypeAttributes();
			newAttributes = FilterTypeAttributes(newAttributes);

			Type baseClass = GetGenericType(typeToIntercept);

			ModuleBuilder moduleBuilder = GetModuleBuilder();
			typeBuilder = moduleBuilder.DefineType(
				"DynamicModule.ns.Wrapped_" + typeToIntercept.Name + "_" + Guid.NewGuid().ToString("N"),
				newAttributes,
				baseClass);

			this.mainTypeMapper = DefineGenericArguments(typeBuilder, baseClass);

			if (this.typeToIntercept.IsGeneric())
			{
				var definition = this.typeToIntercept.GetGenericTypeDefinition();
				var mappedParameters = definition.GetGenericArguments().Select(t => this.mainTypeMapper.Map(t)).ToArray();
				this.targetType = definition.MakeGenericType(mappedParameters);
			}
			else
			{
				this.targetType = this.typeToIntercept;
			}

			proxyInterceptionPipelineField = InterceptingProxyImplementor.ImplementIInterceptingProxy(typeBuilder);
		}

		private static Type GetGenericType(Type typeToIntercept)
		{
			if (typeToIntercept.IsGeneric())
			{
				return typeToIntercept.GetGenericTypeDefinition();
			}
			return typeToIntercept;
		}

		private static GenericParameterMapper DefineGenericArguments(TypeBuilder typeBuilder, Type baseClass)
		{
			if (!baseClass.IsGeneric())
			{
				return GenericParameterMapper.DefaultMapper;
			}
			Type[] genericArguments = baseClass.GetGenericArguments();

			GenericTypeParameterBuilder[] genericTypes = typeBuilder.DefineGenericParameters(
				genericArguments.Select(t => t.Name).ToArray());

			for (int i = 0; i < genericArguments.Length; ++i)
			{
				genericTypes[i].SetGenericParameterAttributes(genericArguments[i].GetGenericParameterAttributes());
				var interfaceConstraints = new List<Type>();
				foreach (Type constraint in genericArguments[i].GetGenericParameterConstraints())
				{
					if (constraint.IsClassType())
					{
						genericTypes[i].SetBaseTypeConstraint(constraint);
					}
					else
					{
						interfaceConstraints.Add(constraint);
					}
				}
				if (interfaceConstraints.Count > 0)
				{
					genericTypes[i].SetInterfaceConstraints(interfaceConstraints.ToArray());
				}
			}

			return new GenericParameterMapper(genericArguments, genericTypes.Cast<Type>().ToArray());
		}

		private static TypeAttributes FilterTypeAttributes(TypeAttributes attributes)
		{
			if ((attributes & TypeAttributes.NestedPublic) != 0)
			{
				attributes &= ~TypeAttributes.NestedPublic;
				attributes |= TypeAttributes.Public;
			}
#if !NETCORE
			attributes &= ~TypeAttributes.ReservedMask;
#endif
			attributes &= ~TypeAttributes.Abstract;

			return attributes;
		}

		private HashSet<Type> GetImplementedInterfacesSet()
		{
			HashSet<Type> implementedInterfaces = new HashSet<Type>();
			AddToImplementedInterfaces(this.typeToIntercept, implementedInterfaces);
			return implementedInterfaces;
		}

		private static void AddToImplementedInterfaces(Type type, HashSet<Type> implementedInterfaces)
		{
			if (!implementedInterfaces.Contains(type))
			{
				if (type.IsInterfaceType())
				{
					implementedInterfaces.Add(type);
				}

				foreach (var @interface in type.GetInterfaces())
				{
					AddToImplementedInterfaces(@interface, implementedInterfaces);
				}
			}
		}
	}
}
