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
using Xunit;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using SF.Core.ServiceManagement;

using SF.Core.Interception;
namespace SF.UT.Interception
{
	public class FeatureTest
    {
		public interface IAdd
		{
			int Add(int a, int b);
		}
		class Add : IAdd
		{
			int IAdd.Add(int a, int b)
			{
				return  a + b;
			}
		}
		class AddBehavor1 : IInterceptionBehavior
		{
			public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
			{
				return getNext()(input, getNext);
			}
		}
		
		[Fact]
        public void TypeConstructorTest()
        {
			var icg = new InterceptingClassGenerator(typeof(object), typeof(IAdd));
			var type=icg.GenerateType();
			var o = Activator.CreateInstance(type);
			var proxy = (IInterceptingProxy)o;
			var add = (IAdd)o;
			proxy.AddInterceptionBehavior(new AddBehavor1());
			proxy.AddInterceptionBehavior(new ProxyInvokerFactory().Create(typeof(IAdd),new Add()));
			var re =add.Add(1, 2);
			Assert.Equal(3, re);
		}

	}
}
