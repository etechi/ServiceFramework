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
	/// <summary>
	/// The InterceptionBehaviorPipeline class encapsulates a list of <see cref="IInterceptionBehavior"/>s
	/// and manages calling them in the proper order with the right inputs.
	/// </summary>
	public class InterceptionBehaviorPipeline
	{
		private readonly List<IInterceptionBehavior> interceptionBehaviors;

		/// <summary>
		/// Creates a new <see cref="HandlerPipeline"/> with an empty pipeline.
		/// </summary>
		public InterceptionBehaviorPipeline()
		{
			interceptionBehaviors = new List<IInterceptionBehavior>();
		}

		/// <summary>
		/// Creates a new <see cref="HandlerPipeline"/> with the given collection
		/// of <see cref="ICallHandler"/>s.
		/// </summary>
		/// <param name="interceptionBehaviors">Collection of interception behaviors to add to the pipeline.</param>
		public InterceptionBehaviorPipeline(IEnumerable<IInterceptionBehavior> interceptionBehaviors)
		{
			Ensure.NotNull(interceptionBehaviors, "interceptionBehaviors");
			this.interceptionBehaviors = new List<IInterceptionBehavior>(interceptionBehaviors);
		}

		/// <summary>
		/// Get the number of interceptors in this pipeline.
		/// </summary>
		public int Count
		{
			get { return interceptionBehaviors.Count; }
		}

		/// <summary>
		/// Execute the pipeline with the given input.
		/// </summary>
		/// <param name="input">Input to the method call.</param>
		/// <param name="target">The ultimate target of the call.</param>
		/// <returns>Return value from the pipeline.</returns>
		public IMethodReturn Invoke(IMethodInvocation input, InvokeInterceptionBehaviorDelegate target)
		{
			if (interceptionBehaviors.Count == 0)
			{
				return target(input, null);
			}

			int interceptorIndex = 0;

			IMethodReturn result = interceptionBehaviors[0].Invoke(input, delegate
			{
				++interceptorIndex;
				if (interceptorIndex < interceptionBehaviors.Count)
				{
					return interceptionBehaviors[interceptorIndex].Invoke;
				}
				else
				{
					return target;
				}
			});
			return result;
		}

		/// <summary>
		/// Adds a <see cref="IInterceptionBehavior"/> to the pipeline.
		/// </summary>
		/// <param name="interceptionBehavior">The interception behavior to add.</param>
		public void Add(IInterceptionBehavior interceptionBehavior)
		{
			Ensure.NotNull(interceptionBehavior, "interceptionBehavior");
			this.interceptionBehaviors.Add(interceptionBehavior);
		}
	}
}
