using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using SF.Core.ServiceManagement;
using SF.Core.DI;
using SF.Core.ServiceManagement.Storages;

namespace SF.UT.DI
{
	

	public class FeatureTest
    {
		interface IInterface<T>
		{
			string ToString(T obj);
		}
		interface IInterface2<T>
		{
			string ToString(T obj);
		}
		class Implement<T> : IInterface<T>, IInterface2<T>
		{
			public string ToString(T obj)
			{
				return obj.ToString();
			}
		}

		class ImplementWithArg<T> : IInterface<T>, IInterface2<T>
		{
			string Prefix { get; }
			public ImplementWithArg(string Prefix)
			{
				this.Prefix = Prefix??"null";
			}
			public string ToString(T obj)
			{
				return Prefix+obj.ToString();
			}
		}

		[Fact]
        public void Generic()
        {
			var sc = new ServiceCollection();
			sc.UseSystemMemoryCache();
			sc.UseMemoryManagedServiceSource();
			sc.AddTransient(typeof(IInterface<>), typeof(Implement<>));
			var sp = sc.BuildServiceResolver();
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
			var sc = new ServiceCollection();
			sc.AddScoped<IMultipleImplTest,MultipleImplTestScope>();
			//sc.AddSingleton<IMultipleImplTest,MultipleImplTestGlobal>();
			sc.UseSystemMemoryCache();
			sc.UseMemoryManagedServiceSource();

			var sp = sc.BuildServiceResolver();
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
			var sc = new ServiceCollection();
			sc.AddTransient<IInterface<int>, Implement<int>>();
			sc.UseSystemMemoryCache();
			sc.UseMemoryManagedServiceSource();
			var sp = sc.BuildServiceResolver();
			var re = sp.Resolve<Func<IInterface<int>>>();
			var r = re();
			Assert.Equal("123", r.ToString(123));
		}
		[Fact]
		public void ByFuncWithId()
		{
			var sc = new ServiceCollection();
			sc.AddTransient<IInterface<int>, Implement<int>>();
			sc.UseSystemMemoryCache();
			sc.UseMemoryManagedServiceSource();
			sc.UseNewtonsoftJson();

			var sp = sc.BuildServiceResolver();
			var cfgs = sp.Resolve<MemoryServiceSource>();
			cfgs.SetConfig<IInterface<int>, Implement<int>>(1,new { });
			var re = sp.Resolve<Func<long, IInterface<int>>>();
			var r = re(1);
			Assert.Equal("123", r.ToString(123));
		}
		[Fact]
		public void ByLazy()
		{
			var sc = new ServiceCollection();
			sc.AddTransient<IInterface<int>, Implement<int>>();
			sc.UseSystemMemoryCache();
			sc.UseMemoryManagedServiceSource();
			var sp = sc.BuildServiceResolver();
			var re = sp.Resolve<Lazy<IInterface<int>>>();
			var r = re.Value;
			Assert.Equal("123", r.ToString(123));

		}
		[Fact]
		public void UnmanagedEnumerable()
		{
			var sc = new ServiceCollection();
			sc.AddTransient<IInterface<int>, Implement<int>>();
			sc.AddTransient<IInterface<int>>((s)=>new Implement<int>());
			sc.UseSystemMemoryCache();
			sc.UseMemoryManagedServiceSource();
			var sp = sc.BuildServiceResolver();
			var re = sp.Resolve<IEnumerable<IInterface<int>>>();

			Assert.Equal(2,re.Count());
			foreach (var i in re)
				Assert.Equal("123", i.ToString(123));

		}
		[Fact]
		public void ManagedEnumerable()
		{
			var sc = new ServiceCollection();
			sc.AddTransient<IInterface<int>, Implement<int>>();
			sc.UseSystemMemoryCache();
			sc.UseMemoryManagedServiceSource();
			sc.UseNewtonsoftJson();

			var sp = sc.BuildServiceResolver();

			var cfgs = sp.Resolve<MemoryServiceSource>();

			cfgs.SetConfig<IInterface<int>, Implement<int>>(1, new { },3);
			cfgs.SetConfig<IInterface<int>, Implement<int>>(2, new { },2);
			cfgs.SetConfig<IInterface<int>, Implement<int>>(3, new { },1);


			var re = sp.Resolve<IEnumerable<IInterface<int>>>();
			Assert.Equal(3, re.Count());
			foreach (var i in re)
				Assert.Equal("123", i.ToString(123));

		}
		[Fact]
		public void InternalServiceExists()
		{
			var sc = new ServiceCollection();
			sc.AddTransient<IInterface<int>, Implement<int>>();
			sc.AddTransient<IInterface<int>, ImplementWithArg<int>>();
			sc.UseSystemMemoryCache();
			sc.UseMemoryManagedServiceSource();
			sc.UseNewtonsoftJson();

			var sp = sc.BuildServiceResolver();

			var cfgs = sp.Resolve<MemoryServiceSource>();

			cfgs.SetConfig<IInterface<int>, ImplementWithArg<int>>(1, new { Prefix = "s1" }, 3);
			cfgs.SetConfig<IInterface<int>, ImplementWithArg<int>>(2, new { Prefix="s2" }, 2, 1);
			cfgs.SetConfig<IInterface<int>, ImplementWithArg<int>>(3, new { Prefix = "s3" }, 1, 1);

			var re = sp.ResolveInternal<IInterface<int>>(1);
			Assert.Equal("s3123", re.ToString(123));
		}
		[Fact]
		public void InternalServiceNotExists()
		{
			var sc = new ServiceCollection();
			sc.AddTransient<IInterface<int>, Implement<int>>();
			sc.AddTransient<IInterface<int>, ImplementWithArg<int>>();
			sc.AddTransient<IInterface2<int>, ImplementWithArg<int>>();
			sc.UseSystemMemoryCache();
			sc.UseMemoryManagedServiceSource();
			sc.UseNewtonsoftJson();

			var sp = sc.BuildServiceResolver();

			var cfgs = sp.Resolve<MemoryServiceSource>();

			cfgs.SetConfig<IInterface<int>, ImplementWithArg<int>>(1, new { Prefix = "s1" }, 3);
			cfgs.SetConfig<IInterface2<int>, ImplementWithArg<int>>(2, new { Prefix = "s2" }, 2, 1);
			cfgs.SetConfig<IInterface2<int>, ImplementWithArg<int>>(3, new { Prefix = "s3" }, 1, 1);

			var re = sp.ResolveInternal<IInterface<int>>(1);
			Assert.Equal("s1123", re.ToString(123));
		}
		[Fact]
		public void InternalServiceNotExists2()
		{
			var sc = new ServiceCollection();
			sc.AddTransient<IInterface<int>, Implement<int>>();
			sc.AddTransient<IInterface<int>, ImplementWithArg<int>>();
			sc.AddTransient<IInterface2<int>, ImplementWithArg<int>>();
			sc.UseSystemMemoryCache();
			sc.UseMemoryManagedServiceSource();
			sc.UseNewtonsoftJson();

			var sp = sc.BuildServiceResolver();

			var cfgs = sp.Resolve<MemoryServiceSource>();

			cfgs.SetConfig<IInterface2<int>, ImplementWithArg<int>>(1, new { Prefix = "s1" }, 3);
			cfgs.SetConfig<IInterface2<int>, ImplementWithArg<int>>(2, new { Prefix = "s2" }, 2, 1);
			cfgs.SetConfig<IInterface2<int>, ImplementWithArg<int>>(3, new { Prefix = "s3" }, 1, 1);

			var re = sp.ResolveInternal<IInterface<int>>(1);
			Assert.Equal("null123", re.ToString(123));
		}
		[Fact]
		public void ServiceConfigChanged()
		{
			var sc = new ServiceCollection();
			sc.AddTransient<IInterface<int>, ImplementWithArg<int>>();
			sc.UseSystemMemoryCache();
			sc.UseMemoryManagedServiceSource();
			sc.UseNewtonsoftJson();

			var sp = sc.BuildServiceResolver();

			var cfgs = sp.Resolve<MemoryServiceSource>();

			cfgs.SetConfig<IInterface<int>, ImplementWithArg<int>>(1, new { Prefix = "s1" });
			var re = sp.ResolveInternal<IInterface<int>>(1);
			Assert.Equal("s1123", re.ToString(123));

			cfgs.SetConfig<IInterface<int>, ImplementWithArg<int>>(1, new { Prefix = "s2" });
			re = sp.ResolveInternal<IInterface<int>>(1);
			Assert.Equal("s2123", re.ToString(123));
		}
		[Fact]
		public void ServicePriorityChanged()
		{
			var sc = new ServiceCollection();
			sc.AddTransient<IInterface<int>, ImplementWithArg<int>>();
			sc.UseSystemMemoryCache();
			sc.UseMemoryManagedServiceSource();
			sc.UseNewtonsoftJson();

			var sp = sc.BuildServiceResolver();

			var cfgs = sp.Resolve<MemoryServiceSource>();

			cfgs.SetConfig<IInterface<int>, ImplementWithArg<int>>(1, new { Prefix = "s1" },1);
			cfgs.SetConfig<IInterface<int>, ImplementWithArg<int>>(2, new { Prefix = "s2" },2);
			var re = sp.Resolve<IInterface<int>>();
			Assert.Equal("s1123", re.ToString(123));

			cfgs.SetConfig<IInterface<int>, ImplementWithArg<int>>(2, new { Prefix = "s2" }, 0);
			re = sp.Resolve<IInterface<int>>();
			Assert.Equal("s2123", re.ToString(123));
		}
	}
}
