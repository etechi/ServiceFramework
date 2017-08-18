using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using SF.Core.ServiceManagement;
using SF.Core.DI;

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
