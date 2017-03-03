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

namespace SF.Core.Interception
{
	/// <summary>
	/// Implementation of <see cref="IMethodInvocation"/> used
	/// by the virtual method interceptor.
	/// </summary>
	public class VirtualMethodInvocation : IMethodInvocation
	{
		private readonly ParameterCollection inputs;
		private readonly ParameterCollection arguments;
		private readonly Dictionary<string, object> context;
		private readonly object target;
		private readonly MethodBase targetMethod;
		public object[] RawArguments { get; }
		/// <summary>
		/// Construct a new <see cref="VirtualMethodInvocation"/> instance for the
		/// given target object and method, passing the <paramref name="parameterValues"/>
		/// to the target method.
		/// </summary>
		/// <param name="target">Object that is target of this invocation.</param>
		/// <param name="targetMethod">Method on <paramref name="target"/> to call.</param>
		/// <param name="parameterValues">Values for the parameters.</param>
		public VirtualMethodInvocation(object target, MethodBase targetMethod, params object[] parameterValues)
		{
			Ensure.NotNull(targetMethod, "targetMethod");

			this.target = target;
			this.targetMethod = targetMethod;
			this.context = new Dictionary<string, object>();
			this.RawArguments = parameterValues;
			ParameterInfo[] targetParameters = targetMethod.GetParameters();
			this.arguments = new ParameterCollection(parameterValues, targetParameters, param => true);
			this.inputs = new ParameterCollection(parameterValues, targetParameters, param => !param.IsOut);
		}

		/// <summary>
		/// Gets the inputs for this call.
		/// </summary>
		public IParameterCollection Inputs
		{
			get { return inputs; }
		}

		/// <summary>
		/// Collection of all parameters to the call: in, out and byref.
		/// </summary>
		public IParameterCollection Arguments
		{
			get { return arguments; }
		}

		/// <summary>
		/// Retrieves a dictionary that can be used to store arbitrary additional
		/// values. This allows the user to pass values between call handlers.
		/// </summary>
		public IDictionary<string, object> InvocationContext
		{
			get { return context; }
		}

		/// <summary>
		/// The object that the call is made on.
		/// </summary>
		public object Target
		{
			get { return target; }
		}

		/// <summary>
		/// The method on Target that we're aiming at.
		/// </summary>
		public MethodBase MethodBase
		{
			get { return targetMethod; }
		}

		/// <summary>
		/// Factory method that creates the correct implementation of
		/// IMethodReturn.
		/// </summary>
		/// <param name="returnValue">Return value to be placed in the IMethodReturn object.</param>
		/// <param name="outputs">All arguments passed or returned as out/byref to the method. 
		/// Note that this is the entire argument list, including in parameters.</param>
		/// <returns>New IMethodReturn object.</returns>
		public IMethodReturn CreateMethodReturn(object returnValue, params object[] outputs)
		{
			return new VirtualMethodReturn(this, returnValue, outputs);
		}

		/// <summary>
		/// Factory method that creates the correct implementation of
		/// IMethodReturn in the presence of an exception.
		/// </summary>
		/// <param name="ex">Exception to be set into the returned object.</param>
		/// <returns>New IMethodReturn object</returns>
		public IMethodReturn CreateExceptionMethodReturn(Exception ex)
		{
			return new VirtualMethodReturn(this, ex);
		}
	}

	/// <summary>
	/// An implementation of <see cref="IMethodReturn"/> used by
	/// the virtual method interception mechanism.
	/// </summary>
	public class VirtualMethodReturn : IMethodReturn
	{
		private ParameterCollection outputs;
		private Exception exception;
		private IDictionary<string, object> invocationContext;
		private object returnValue;

		/// <summary>
		/// Construct a <see cref="VirtualMethodReturn"/> instance that returns
		/// a value.
		/// </summary>
		/// <param name="originalInvocation">The method invocation.</param>
		/// <param name="returnValue">Return value (should be null if method returns void).</param>
		/// <param name="arguments">All arguments (including current values) passed to the method.</param>
		public VirtualMethodReturn(IMethodInvocation originalInvocation, object returnValue, object[] arguments)
		{
			Ensure.NotNull(originalInvocation, "originalInvocation");

			invocationContext = originalInvocation.InvocationContext;
			this.returnValue = returnValue;
			outputs = new ParameterCollection(arguments, originalInvocation.MethodBase.GetParameters(),
				delegate (ParameterInfo pi) { return pi.ParameterType.IsByRef; });
		}

		/// <summary>
		/// Construct a <see cref="VirtualMethodReturn"/> instance for when the target method throws an exception.
		/// </summary>
		/// <param name="originalInvocation">The method invocation.</param>
		/// <param name="exception">Exception that was thrown.</param>
		public VirtualMethodReturn(IMethodInvocation originalInvocation, Exception exception)
		{
			Ensure.NotNull(originalInvocation, "originalInvocation");

			invocationContext = originalInvocation.InvocationContext;
			this.exception = exception;
			outputs = new ParameterCollection(new object[0], new ParameterInfo[0], delegate { return false; });
		}

		/// <summary>
		/// The collection of output parameters. If the method has no output
		/// parameters, this is a zero-length list (never null).
		/// </summary>
		public IParameterCollection Outputs
		{
			get { return outputs; }
		}

		/// <summary>
		/// Returns value from the method call.
		/// </summary>
		/// <remarks>This value is null if the method has no return value.</remarks>
		public object ReturnValue
		{
			get { return returnValue; }
			set { returnValue = value; }
		}

		/// <summary>
		/// If the method threw an exception, the exception object is here.
		/// </summary>
		public Exception Exception
		{
			get { return exception; }
			set { exception = value; }
		}

		/// <summary>
		/// Retrieves a dictionary that can be used to store arbitrary additional
		/// values. This allows the user to pass values between call handlers.
		/// </summary>
		/// <remarks>This is guaranteed to be the same dictionary that was used
		/// in the IMethodInvocation object, so handlers can set context
		/// properties in the pre-call phase and retrieve them in the after-call phase.
		/// </remarks>
		public IDictionary<string, object> InvocationContext
		{
			get { return invocationContext; }
		}
	}
}
