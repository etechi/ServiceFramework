using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using Newtonsoft.Json;
using SF.Services.Management;
using SF.DI;
using SF.DI.Microsoft;

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
			var sc = new ServiceCollection();
			sc.AddTransient(typeof(IInterface<>), typeof(Implement<>));
			var sp = sc.BuildServiceProvider();
			var re = sp.GetService<IInterface<int>>();
			Assert.Equal("123", re.ToString(123));
		}
		[Fact]
		public void ByFunc()
		{
			var sc = new ServiceCollection();
			sc.AddTransient<IInterface<int>, Implement<int>>();
			var sp = sc.BuildServiceProvider();
			var re = sp.GetService<Func<IInterface<int>>>();
			var r = re();
			Assert.Equal("123", r.ToString(123));

		}
		[Fact]
		public void ByLazy()
		{
			var sc = new ServiceCollection();
			sc.AddTransient<IInterface<int>, Implement<int>>();
			var sp = sc.BuildServiceProvider();
			var re = sp.GetService<Lazy<IInterface<int>>>();
			var r = re.Value;
			Assert.Equal("123", r.ToString(123));

		}
	}
}
