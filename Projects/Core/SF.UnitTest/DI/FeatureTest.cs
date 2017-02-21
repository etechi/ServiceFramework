using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using SF.Core.ManagedServices;
using SF.Core.DI;
using SF.Core.DI.MicrosoftExtensions;

namespace SF.UT.DI
{
	

	public class FeatureTest
    {
		interface IInterface<T>
		{
			string ToString(T obj);
		}
		class Implement<T> : IInterface<T>
		{
			public string ToString(T obj)
			{
				return obj.ToString();
			}
		}
		
        [Fact]
        public void Generic()
        {
			var sc = DIServiceCollection.Create();
			sc.AddTransient(typeof(IInterface<>), typeof(Implement<>));
			var sp = sc.BuildServiceProvider();
			var re = sp.Resolve<IInterface<int>>();
			Assert.Equal("123", re.ToString(123));
		}
		public interface IMultipleImplTest
		{
			string Text();
		}
		public class MultipleImplTestGlobal : IMultipleImplTest
		{
			public string Text()
			{
				return "Global Text";
			}
		}
		public class MultipleImplTestScope : IMultipleImplTest
		{
			int idx = 0;
			public string Text()
			{
				return "Scope Text "+(idx++);
			}
		}
		[Fact]
		public void ScopeWithMultipleImplements()
		{
			var sc = DIServiceCollection.Create();
			sc.AddScoped<IMultipleImplTest,MultipleImplTestScope>();
			//sc.AddSingleton<IMultipleImplTest,MultipleImplTestGlobal>();

			var sp = sc.BuildServiceProvider();
			var re = sp.Resolve<IMultipleImplTest>();
			Assert.Equal("Scope Text 0", re.Text());
			sp.WithScope((isp) =>
			{
				var re1 = isp.Resolve<IMultipleImplTest>();
				Assert.Equal("Scope Text 0", re1.Text());
			});
			Assert.Equal("Scope Text 1", re.Text());
			sp.WithScope((isp) =>
			{
				var re1 = isp.Resolve<IMultipleImplTest>();
				Assert.Equal("Scope Text 0", re1.Text());
			});
			Assert.Equal("Scope Text 2", re.Text());
		}
		[Fact]
		public void ByFunc()
		{
			var sc = DIServiceCollection.Create();
			sc.AddTransient<IInterface<int>, Implement<int>>();
			var sp = sc.BuildServiceProvider();
			var re = sp.Resolve<Func<IInterface<int>>>();
			var r = re();
			Assert.Equal("123", r.ToString(123));

		}
		[Fact]
		public void ByLazy()
		{
			var sc = DIServiceCollection.Create();
			sc.AddTransient<IInterface<int>, Implement<int>>();
			var sp = sc.BuildServiceProvider();
			var re = sp.Resolve<Lazy<IInterface<int>>>();
			var r = re.Value;
			Assert.Equal("123", r.ToString(123));

		}
	}
}
