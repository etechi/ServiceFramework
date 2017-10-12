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
using System.Runtime.CompilerServices;
using System.Collections;

namespace SF.Core.Interception
{
	internal static class CompilerGeneratedAttributeMethods
	{
		public static ConstructorInfo CompilerGeneratedAttribute
		{
			get { return StaticReflection.GetConstructorInfo(() => new CompilerGeneratedAttribute()); }
		}
	}
	internal static class IMethodInvocationMethods
	{
		internal static MethodInfo CreateExceptionMethodReturn
		{
			get { return StaticReflection.GetMethodInfo((IMethodInvocation mi) => mi.CreateExceptionMethodReturn(default(Exception))); }
		}

		internal static MethodInfo CreateReturn
		{
			// Using static reflection causes an FxCop rule to throw an exception here, using plain old reflection instead
			// logged as https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=485609
			get { return typeof(IMethodInvocation).GetMethod("CreateMethodReturn"); }
		}

		internal static MethodInfo GetArguments
		{
			get { return StaticReflection.GetPropertyGetMethodInfo((IMethodInvocation mi) => mi.Arguments); }
		}
	}
	internal static class IListMethods
	{
		internal static MethodInfo GetItem
		{
			get { return typeof(IList).GetProperty("Item").GetGetMethod(); }
		}
	}
	internal static class MethodBaseMethods
	{
		internal static MethodInfo GetMethodFromHandle
		{
			get
			{
				return StaticReflection.GetMethodInfo(
					() => MethodBase.GetMethodFromHandle(default(RuntimeMethodHandle)));
			}
		}

		internal static MethodInfo GetMethodForGenericFromHandle
		{
			get
			{
				return StaticReflection.GetMethodInfo(
					() => MethodBase.GetMethodFromHandle(default(RuntimeMethodHandle), default(RuntimeTypeHandle)));
			}
		}
	}
	internal static class VirtualMethodInvocationMethods
	{
		internal static ConstructorInfo VirtualMethodInvocation
		{
			get
			{
				return StaticReflection.GetConstructorInfo(
					() => new VirtualMethodInvocation(default(object), default(MethodBase)));
			}
		}
	}
	internal static class IMethodReturnMethods
	{
		internal static MethodInfo GetException
		{
			get { return StaticReflection.GetPropertyGetMethodInfo((IMethodReturn imr) => imr.Exception); }
		}

		internal static MethodInfo GetReturnValue
		{
			get { return StaticReflection.GetPropertyGetMethodInfo((IMethodReturn imr) => imr.ReturnValue); }
		}

		internal static MethodInfo GetOutputs
		{
			get { return StaticReflection.GetPropertyGetMethodInfo((IMethodReturn imr) => imr.Outputs); }
		}
	}
	/// <summary>
	/// A helper class that encapsulates two different
	/// data items together into a a single item.
	/// </summary>
	public class Pair<TFirst, TSecond>
	{
		private TFirst first;
		private TSecond second;

		/// <summary>
		/// Create a new <see cref="Pair{TFirst, TSecond}"/> containing
		/// the two values give.
		/// </summary>
		/// <param name="first">First value</param>
		/// <param name="second">Second value</param>
		public Pair(TFirst first, TSecond second)
		{
			this.first = first;
			this.second = second;
		}

		/// <summary>
		/// The first value of the pair.
		/// </summary>
		public TFirst First
		{
			get { return first; }
		}

		/// <summary>
		/// The second value of the pair.
		/// </summary>
		public TSecond Second
		{
			get { return second; }
		}
	}

	/// <summary>
	/// Container for a Pair helper method.
	/// </summary>
	public static class Pair
	{
		/// <summary>
		/// A helper factory method that lets users take advantage of type inference.
		/// </summary>
		/// <typeparam name="TFirstParameter">Type of first value.</typeparam>
		/// <typeparam name="TSecondParameter">Type of second value.</typeparam>
		/// <param name="first">First value.</param>
		/// <param name="second">Second value.</param>
		/// <returns>A new <see cref="Pair{TFirstParameter, TSecondParameter}"/> instance.</returns>
		public static Pair<TFirstParameter, TSecondParameter> Make<TFirstParameter, TSecondParameter>(TFirstParameter first, TSecondParameter second)
		{
			return new Pair<TFirstParameter, TSecondParameter>(first, second);
		}
	}
	/// <summary>
	/// A series of helper methods to deal with sequences -
	/// objects that implement <see cref="IEnumerable{T}"/>.
	/// </summary>
	public static class Sequence
	{
		/// <summary>
		/// A function that turns an arbitrary parameter list into an
		/// <see cref="IEnumerable{T}"/>.
		/// </summary>
		/// <typeparam name="T">Type of arguments.</typeparam>
		/// <param name="arguments">The items to put into the collection.</param>
		/// <returns>An array that contains the values of the <paramref name="arguments"/>.</returns>
		public static T[] Collect<T>(params T[] arguments)
		{
			return arguments;
		}

		/// <summary>
		/// Given two sequences, return a new sequence containing the corresponding values
		/// from each one.
		/// </summary>
		/// <typeparam name="TFirstSequenceElement">Type of first sequence.</typeparam>
		/// <typeparam name="TSecondSequenceElement">Type of second sequence.</typeparam>
		/// <param name="sequence1">First sequence of items.</param>
		/// <param name="sequence2">Second sequence of items.</param>
		/// <returns>New sequence of pairs. This sequence ends when the shorter of sequence1 and sequence2 does.</returns>
		public static IEnumerable<Pair<TFirstSequenceElement, TSecondSequenceElement>> Zip<TFirstSequenceElement, TSecondSequenceElement>(IEnumerable<TFirstSequenceElement> sequence1, IEnumerable<TSecondSequenceElement> sequence2)
		{
			var enum1 = sequence1.GetEnumerator();
			var enum2 = sequence2.GetEnumerator();

			while (enum1.MoveNext())
			{
				if (enum2.MoveNext())
				{
					yield return new Pair<TFirstSequenceElement, TSecondSequenceElement>(enum1.Current, enum2.Current);
				}
				else
				{
					yield break;
				}
			}
		}
	}
	internal static class InvokeInterceptionBehaviorDelegateMethods
	{
		internal static ConstructorInfo InvokeInterceptionBehaviorDelegate
		{
			get
			{
				// cannot use static reflection with delegate types
				return typeof(InvokeInterceptionBehaviorDelegate)
					.GetConstructor(Sequence.Collect(typeof(object), typeof(IntPtr)));
			}
		}
	}

	internal static class InterceptionBehaviorPipelineMethods
	{
		internal static ConstructorInfo Constructor
		{
			get { return StaticReflection.GetConstructorInfo(() => new InterceptionBehaviorPipeline()); }
		}

		internal static MethodInfo Add
		{
			get { return StaticReflection.GetMethodInfo((InterceptionBehaviorPipeline pip) => pip.Add(null)); }
		}

		internal static MethodInfo Invoke
		{
			get { return StaticReflection.GetMethodInfo((InterceptionBehaviorPipeline pip) => pip.Invoke(null, null)); }
		}
	}

	/// <summary>
	/// Represents the implementation of an interface method.
	/// </summary>
	public class InterfaceMethodOverride
	{
		private static readonly MethodInfo BuildAdditionalInterfaceNonImplementedExceptionMethod =
			StaticReflection.GetMethodInfo(() => InterfaceMethodOverride.BuildAdditionalInterfaceNonImplementedException());

		private const MethodAttributes ImplicitImplementationAttributes =
			MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final
			| MethodAttributes.HideBySig | MethodAttributes.NewSlot;
		private const MethodAttributes ExplicitImplementationAttributes =
			MethodAttributes.Private | MethodAttributes.Virtual | MethodAttributes.Final
			| MethodAttributes.HideBySig | MethodAttributes.NewSlot;

		private readonly TypeBuilder typeBuilder;
		private readonly MethodInfo methodToOverride;
		private readonly ParameterInfo[] methodParameters;
		private readonly FieldBuilder proxyInterceptionPipelineField;
		private readonly bool explicitImplementation;
		private readonly FieldBuilder targetField;
		private readonly Type targetInterface;
		private readonly GenericParameterMapper targetInterfaceParameterMapper;
		private readonly int overrideCount;

		internal InterfaceMethodOverride(
			TypeBuilder typeBuilder,
			FieldBuilder proxyInterceptionPipelineField,
			FieldBuilder targetField,
			MethodInfo methodToOverride,
			Type targetInterface,
			GenericParameterMapper targetInterfaceParameterMapper,
			bool explicitImplementation,
			int overrideCount)
		{
			this.typeBuilder = typeBuilder;
			this.proxyInterceptionPipelineField = proxyInterceptionPipelineField;
			this.explicitImplementation = explicitImplementation;
			this.targetField = targetField;
			this.methodToOverride = methodToOverride;
			this.targetInterface = targetInterface;
			this.targetInterfaceParameterMapper = targetInterfaceParameterMapper;
			this.methodParameters = methodToOverride.GetParameters();
			this.overrideCount = overrideCount;
		}

		internal MethodBuilder AddMethod()
		{
			MethodBuilder delegateMethod = CreateDelegateImplementation();
			return CreateMethodOverride(delegateMethod);
		}

		private string CreateMethodName(string purpose)
		{
			return "<" + methodToOverride.Name + "_" + purpose + ">__" +
				overrideCount.ToString();
		}

		private static readonly OpCode[] LoadArgsOpcodes =
		{
			OpCodes.Ldarg_1,
			OpCodes.Ldarg_2,
			OpCodes.Ldarg_3
		};

		private static void EmitLoadArgument(ILGenerator il, int argumentNumber)
		{
			if (argumentNumber < LoadArgsOpcodes.Length)
			{
				il.Emit(LoadArgsOpcodes[argumentNumber]);
			}
			else
			{
				il.Emit(OpCodes.Ldarg, argumentNumber + 1);
			}
		}

		private static readonly OpCode[] LoadConstOpCodes =
		{
			OpCodes.Ldc_I4_0,
			OpCodes.Ldc_I4_1,
			OpCodes.Ldc_I4_2,
			OpCodes.Ldc_I4_3,
			OpCodes.Ldc_I4_4,
			OpCodes.Ldc_I4_5,
			OpCodes.Ldc_I4_6,
			OpCodes.Ldc_I4_7,
			OpCodes.Ldc_I4_8,
		};

		private static void EmitLoadConstant(ILGenerator il, int i)
		{
			if (i < LoadConstOpCodes.Length)
			{
				il.Emit(LoadConstOpCodes[i]);
			}
			else
			{
				il.Emit(OpCodes.Ldc_I4, i);
			}
		}

		private static void EmitBox(ILGenerator il, Type typeOnStack)
		{
			if (typeOnStack.IsValue() || typeOnStack.IsGenericParameter)
			{
				il.Emit(OpCodes.Box, typeOnStack);
			}
		}

		private static void EmitUnboxOrCast(ILGenerator il, Type targetType)
		{
			if (targetType.IsValue() || targetType.IsGenericParameter)
			{
				il.Emit(OpCodes.Unbox_Any, targetType);
			}
			else
			{
				il.Emit(OpCodes.Castclass, targetType);
			}
		}

		private MethodBuilder CreateDelegateImplementation()
		{
			string methodName = CreateMethodName("DelegateImplementation");

			MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodName,
				MethodAttributes.Private | MethodAttributes.HideBySig);
			List<LocalBuilder> outOrRefLocals = new List<LocalBuilder>();

			var paramMapper = new MethodOverrideParameterMapper(methodToOverride);
			paramMapper.SetupParameters(methodBuilder, this.targetInterfaceParameterMapper);

			methodBuilder.SetReturnType(typeof(IMethodReturn));
			// Adding parameters
			methodBuilder.SetParameters(typeof(IMethodInvocation), typeof(GetNextInterceptionBehaviorDelegate));
			// Parameter 
			methodBuilder.DefineParameter(1, ParameterAttributes.None, "inputs");
			// Parameter 
			methodBuilder.DefineParameter(2, ParameterAttributes.None, "getNext");

			methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(CompilerGeneratedAttributeMethods.CompilerGeneratedAttribute, new object[0]));

			ILGenerator il = methodBuilder.GetILGenerator();

			if (this.targetField != null)
			{
				// forwarding implementation
				Label done = il.DefineLabel();
				LocalBuilder ex = il.DeclareLocal(typeof(Exception));

				LocalBuilder baseReturn = null;
				LocalBuilder parameters = null;

				if (MethodHasReturnValue)
				{
					baseReturn = il.DeclareLocal(paramMapper.GetReturnType());
				}
				LocalBuilder retval = il.DeclareLocal(typeof(IMethodReturn));

				il.BeginExceptionBlock();
				// Call the target method
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, targetField);

				if (methodParameters.Length > 0)
				{
					parameters = il.DeclareLocal(typeof(IParameterCollection));
					il.Emit(OpCodes.Ldarg_1);
					il.EmitCall(OpCodes.Callvirt, IMethodInvocationMethods.GetArguments, null);
					il.Emit(OpCodes.Stloc, parameters);

					for (int i = 0; i < methodParameters.Length; ++i)
					{
						il.Emit(OpCodes.Ldloc, parameters);
						EmitLoadConstant(il, i);
						il.EmitCall(OpCodes.Callvirt, IListMethods.GetItem, null);
						Type parameterType = paramMapper.GetParameterType(methodParameters[i].ParameterType);

						if (parameterType.IsByRef)
						{
							Type elementType = parameterType.GetElementType();
							LocalBuilder refShadowLocal = il.DeclareLocal(elementType);
							outOrRefLocals.Add(refShadowLocal);
							EmitUnboxOrCast(il, elementType);
							il.Emit(OpCodes.Stloc, refShadowLocal);
							il.Emit(OpCodes.Ldloca, refShadowLocal);
						}
						else
						{
							EmitUnboxOrCast(il, parameterType);
						}
					}
				}

				MethodInfo callTarget = methodToOverride;
				if (callTarget.IsGenericMethod)
				{
					callTarget = methodToOverride.MakeGenericMethod(paramMapper.GenericMethodParameters);
				}

				il.Emit(OpCodes.Callvirt, callTarget);

				if (MethodHasReturnValue)
				{
					il.Emit(OpCodes.Stloc, baseReturn);
				}

				// Generate  the return value
				il.Emit(OpCodes.Ldarg_1);
				if (MethodHasReturnValue)
				{
					il.Emit(OpCodes.Ldloc, baseReturn);
					EmitBox(il, paramMapper.GetReturnType());
				}
				else
				{
					il.Emit(OpCodes.Ldnull);
				}
				EmitLoadConstant(il, methodParameters.Length);
				il.Emit(OpCodes.Newarr, typeof(object));

				if (methodParameters.Length > 0)
				{
					LocalBuilder outputArguments = il.DeclareLocal(typeof(object[]));
					il.Emit(OpCodes.Stloc, outputArguments);

					int outputArgNum = 0;
					for (int i = 0; i < methodParameters.Length; ++i)
					{
						il.Emit(OpCodes.Ldloc, outputArguments);
						EmitLoadConstant(il, i);

						Type parameterType = paramMapper.GetParameterType(methodParameters[i].ParameterType);
						if (parameterType.IsByRef)
						{
							parameterType = parameterType.GetElementType();
							il.Emit(OpCodes.Ldloc, outOrRefLocals[outputArgNum++]);
							EmitBox(il, parameterType);
						}
						else
						{
							il.Emit(OpCodes.Ldloc, parameters);
							EmitLoadConstant(il, i);
							il.Emit(OpCodes.Callvirt, IListMethods.GetItem);
						}
						il.Emit(OpCodes.Stelem_Ref);
					}
					il.Emit(OpCodes.Ldloc, outputArguments);
				}

				il.Emit(OpCodes.Callvirt, IMethodInvocationMethods.CreateReturn);
				il.Emit(OpCodes.Stloc, retval);
				il.BeginCatchBlock(typeof(Exception));
				il.Emit(OpCodes.Stloc, ex);
				// Create an exception return
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldloc, ex);
				il.EmitCall(OpCodes.Callvirt, IMethodInvocationMethods.CreateExceptionMethodReturn, null);
				il.Emit(OpCodes.Stloc, retval);
				il.EndExceptionBlock();
				il.MarkLabel(done);
				il.Emit(OpCodes.Ldloc, retval);
				il.Emit(OpCodes.Ret);
			}
			else
			{
				// exception-throwing implementation
				il.Emit(OpCodes.Ldarg_1);
				il.EmitCall(OpCodes.Call, BuildAdditionalInterfaceNonImplementedExceptionMethod, null);
				il.EmitCall(OpCodes.Callvirt, IMethodInvocationMethods.CreateExceptionMethodReturn, null);
				il.Emit(OpCodes.Ret);
			}
			return methodBuilder;
		}

		private MethodBuilder CreateMethodOverride(MethodBuilder delegateMethod)
		{
			string methodName =
				this.explicitImplementation
						? methodToOverride.DeclaringType.Name + "." + methodToOverride.Name
						: methodToOverride.Name;

			MethodBuilder methodBuilder =
				typeBuilder.DefineMethod(
					methodName,
					this.explicitImplementation ? ExplicitImplementationAttributes : ImplicitImplementationAttributes);

			var paramMapper = new MethodOverrideParameterMapper(methodToOverride);
			paramMapper.SetupParameters(methodBuilder, this.targetInterfaceParameterMapper);

			methodBuilder.SetReturnType(paramMapper.GetReturnType());
			methodBuilder.SetParameters(methodParameters.Select(pi => paramMapper.GetParameterType(pi.ParameterType)).ToArray());
			if (this.explicitImplementation)
			{
				this.typeBuilder.DefineMethodOverride(methodBuilder, this.methodToOverride);
			}

			int paramNum = 1;
			foreach (ParameterInfo pi in methodParameters)
			{
				methodBuilder.DefineParameter(paramNum++, pi.Attributes, pi.Name);
			}

			ILGenerator il = methodBuilder.GetILGenerator();

			LocalBuilder methodReturn = il.DeclareLocal(typeof(IMethodReturn));
			LocalBuilder ex = il.DeclareLocal(typeof(Exception));
			LocalBuilder parameterArray = il.DeclareLocal(typeof(object[]));
			LocalBuilder inputs = il.DeclareLocal(typeof(VirtualMethodInvocation));

			// Create instance of VirtualMethodInvocation
			il.Emit(OpCodes.Ldarg_0); // target object

			// If we have a targetField, that means we're building a proxy and
			// should use it as the target object. If we don't, we're building
			// a type interceptor and should leave the this pointer as the
			// target.
			if (targetField != null)
			{
				il.Emit(OpCodes.Ldfld, targetField);
			}

			// If we have a generic method, we want to make sure we're using the open constructed generic method
			// so when a closed generic version of the method is invoked the actual type parameters are used
			il.Emit(
				OpCodes.Ldtoken,
				methodToOverride.IsGenericMethodDefinition
					? methodToOverride.MakeGenericMethod(paramMapper.GenericMethodParameters)
					: methodToOverride);
			if (methodToOverride.DeclaringType.IsGeneric())
			{
				// if the declaring type is generic, we need to get the method from the target type
				il.Emit(OpCodes.Ldtoken, targetInterface);
				il.Emit(OpCodes.Call, MethodBaseMethods.GetMethodForGenericFromHandle);
			}
			else
			{
				il.Emit(OpCodes.Call, MethodBaseMethods.GetMethodFromHandle); // target method
			}

			EmitLoadConstant(il, methodParameters.Length);
			il.Emit(OpCodes.Newarr, typeof(object)); // object[] parameters
			if (methodParameters.Length > 0)
			{
				il.Emit(OpCodes.Stloc, parameterArray);

				for (int i = 0; i < methodParameters.Length; ++i)
				{
					il.Emit(OpCodes.Ldloc, parameterArray);
					EmitLoadConstant(il, i);
					EmitLoadArgument(il, i);
					Type elementType = paramMapper.GetParameterType(methodParameters[i].ParameterType);
					if (elementType.IsByRef)
					{
						elementType = paramMapper.GetElementType(methodParameters[i].ParameterType);
						il.Emit(OpCodes.Ldobj, elementType);
					}
					EmitBox(il, elementType);
					il.Emit(OpCodes.Stelem_Ref);
				}

				il.Emit(OpCodes.Ldloc, parameterArray);
			}
			il.Emit(OpCodes.Newobj, VirtualMethodInvocationMethods.VirtualMethodInvocation);
			il.Emit(OpCodes.Stloc, inputs);

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, proxyInterceptionPipelineField);
			il.Emit(OpCodes.Ldloc, inputs);

			// Put delegate reference onto the stack
			il.Emit(OpCodes.Ldarg_0);

			MethodInfo callTarget = delegateMethod;
			if (callTarget.IsGenericMethod)
			{
				callTarget = delegateMethod.MakeGenericMethod(paramMapper.GenericMethodParameters);
			}

			il.Emit(OpCodes.Ldftn, callTarget);

			il.Emit(OpCodes.Newobj, InvokeInterceptionBehaviorDelegateMethods.InvokeInterceptionBehaviorDelegate);

			// And call the pipeline
			il.Emit(OpCodes.Call, InterceptionBehaviorPipelineMethods.Invoke);

			il.Emit(OpCodes.Stloc, methodReturn);

			// Was there an exception?
			Label noException = il.DefineLabel();
			il.Emit(OpCodes.Ldloc, methodReturn);
			il.EmitCall(OpCodes.Callvirt, IMethodReturnMethods.GetException, null);
			il.Emit(OpCodes.Stloc, ex);
			il.Emit(OpCodes.Ldloc, ex);
			il.Emit(OpCodes.Ldnull);
			il.Emit(OpCodes.Ceq);
			il.Emit(OpCodes.Brtrue_S, noException);
			il.Emit(OpCodes.Ldloc, ex);
			il.Emit(OpCodes.Throw);

			il.MarkLabel(noException);

			// Unpack any ref/out parameters
			if (methodParameters.Length > 0)
			{
				int outputArgNum = 0;
				for (paramNum = 0; paramNum < methodParameters.Length; ++paramNum)
				{
					ParameterInfo pi = methodParameters[paramNum];
					if (pi.ParameterType.IsByRef)
					{
						// Get the original parameter value - address of the ref or out
						EmitLoadArgument(il, paramNum);

						// Get the value of this output parameter out of the Outputs collection
						il.Emit(OpCodes.Ldloc, methodReturn);
						il.Emit(OpCodes.Callvirt, IMethodReturnMethods.GetOutputs);
						EmitLoadConstant(il, outputArgNum++);
						il.Emit(OpCodes.Callvirt, IListMethods.GetItem);
						EmitUnboxOrCast(il, paramMapper.GetElementType(pi.ParameterType));

						// And store in the caller
						il.Emit(OpCodes.Stobj, paramMapper.GetElementType(pi.ParameterType));
					}
				}
			}

			if (MethodHasReturnValue)
			{
				il.Emit(OpCodes.Ldloc, methodReturn);
				il.EmitCall(OpCodes.Callvirt, IMethodReturnMethods.GetReturnValue, null);
				EmitUnboxOrCast(il, paramMapper.GetReturnType());
			}
			il.Emit(OpCodes.Ret);

			return methodBuilder;
		}

		private bool MethodHasReturnValue
		{
			get { return methodToOverride.ReturnType != typeof(void); }
		}

		/// <summary>
		/// Used to throw an <see cref="NotImplementedException"/> for non-implemented methods on the
		/// additional interfaces.
		/// </summary>
		public static Exception BuildAdditionalInterfaceNonImplementedException()
		{
			return new NotImplementedException("ExceptionAdditionalInterfaceNotImplemented");
		}
	}
}
