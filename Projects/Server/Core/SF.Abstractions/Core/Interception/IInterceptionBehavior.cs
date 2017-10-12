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
using System.Diagnostics.CodeAnalysis;

namespace SF.Core.Interception
{
	/// <summary>
	/// Interception behaviors implement this interface and are called for each
	/// invocation of the pipelines that they're included in.
	/// </summary>
	public interface IInterceptionBehavior
	{
		/// <summary>
		/// Implement this method to execute your behavior processing.
		/// </summary>
		/// <param name="input">Inputs to the current call to the target.</param>
		/// <param name="getNext">Delegate to execute to get the next delegate in the behavior chain.</param>
		/// <returns>Return value from the target.</returns>
		IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext);

		///// <summary>
		///// Returns the interfaces required by the behavior for the objects it intercepts.
		///// </summary>
		///// <returns>The required interfaces.</returns>
		//[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Could require computations")]
		//IEnumerable<Type> GetRequiredInterfaces();

		///// <summary>
		///// Returns a flag indicating if this behavior will actually do anything when invoked.
		///// </summary>
		///// <remarks>This is used to optimize interception. If the behaviors won't actually
		///// do anything (for example, PIAB where no policies match) then the interception
		///// mechanism can be skipped completely.</remarks>
		//bool WillExecute { get; }
	}

	/// <summary>
	/// This delegate type is the type that points to the next
	/// method to execute in the current pipeline.
	/// </summary>
	/// <param name="input">Inputs to the current method call.</param>
	/// <param name="getNext">Delegate to get the next interceptor in the chain.</param>
	/// <returns>Return from the next method in the chain.</returns>
	[SuppressMessage("Microsoft.Naming", "CA1711", Justification = "A delegate is indeed required.")]
	public delegate IMethodReturn InvokeInterceptionBehaviorDelegate(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext);

	/// <summary>
	/// This delegate type is passed to each interceptor's Invoke method.
	/// Call the delegate to get the next delegate to call to continue
	/// the chain.
	/// </summary>
	/// <returns>Next delegate in the interceptor chain to call.</returns>
	[SuppressMessage("Microsoft.Naming", "CA1711", Justification = "A delegate is indeed required.")]
	public delegate InvokeInterceptionBehaviorDelegate GetNextInterceptionBehaviorDelegate();
}
