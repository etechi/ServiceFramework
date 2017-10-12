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
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
namespace SF.Core.Interception
{
	/// <summary>
	/// This class handles parameter type mapping. When we generate
	/// a generic method, we need to make sure our parameter type
	/// objects line up with the generic parameters on the generated
	/// method, not on the one we're overriding. 
	/// </summary>
	internal class MethodOverrideParameterMapper
	{
		private readonly MethodInfo methodToOverride;
		private GenericParameterMapper genericParameterMapper;

		public MethodOverrideParameterMapper(MethodInfo methodToOverride)
		{
			this.methodToOverride = methodToOverride;
		}

		public void SetupParameters(MethodBuilder methodBuilder, GenericParameterMapper parentMapper)
		{
			if (methodToOverride.IsGenericMethod)
			{
				var genericArguments = methodToOverride.GetGenericArguments();
				var names = genericArguments.Select(t => t.Name).ToArray();
				var builders = methodBuilder.DefineGenericParameters(names);
				for (int i = 0; i < genericArguments.Length; ++i)
				{
					builders[i].SetGenericParameterAttributes(genericArguments[i].GetGenericParameterAttributes());

					var constraintTypes =
						genericArguments[i]
							.GetGenericParameterConstraints()
							.Select(ct => parentMapper.Map(ct))
							.ToArray();

					var interfaceConstraints = constraintTypes.Where(t => t.IsInterfaceType()).ToArray();
					Type baseConstraint = constraintTypes.Where(t => !t.IsInterfaceType()).FirstOrDefault();
					if (baseConstraint != null)
					{
						builders[i].SetBaseTypeConstraint(baseConstraint);
					}
					if (interfaceConstraints.Length > 0)
					{
						builders[i].SetInterfaceConstraints(interfaceConstraints);
					}
				}

				this.genericParameterMapper =
					new GenericParameterMapper(genericArguments, builders.Cast<Type>().ToArray(), parentMapper);
			}
			else
			{
				this.genericParameterMapper = parentMapper;
			}
		}

		public Type GetParameterType(Type originalParameterType)
		{
			return this.genericParameterMapper.Map(originalParameterType);
		}

		public Type GetElementType(Type originalParameterType)
		{
			return GetParameterType(originalParameterType).GetElementType();
		}

		public Type GetReturnType()
		{
			return GetParameterType(methodToOverride.ReturnType);
		}

		public Type[] GenericMethodParameters
		{
			get { return this.genericParameterMapper.GetGeneratedParameters(); }
		}
	}
}
