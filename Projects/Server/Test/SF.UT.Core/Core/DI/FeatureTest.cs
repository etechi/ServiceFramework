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
using SF.Sys.Services;
using SF.Sys.Services.Storages;

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
			sc.AddMicrosoftMemoryCacheAsLocalCache();
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
			sc.AddMicrosoftMemoryCacheAsLocalCache();
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
			sc.AddMicrosoftMemoryCacheAsLocalCache();
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
			sc.AddManagedService();
			sc.AddMicrosoftMemoryCacheAsLocalCache();
			sc.UseMemoryManagedServiceSource();
			sc.AddNewtonsoftJson();

			var sp = sc.BuildServiceResolver();
			var cfgs = sp.Resolve<MemoryServiceSource>();
			cfgs.SetConfig<IInterface<int>, Implement<int>>(1,new { });
			var re = sp.Resolve<TypedInstanceResolver<IInterface<int>>>();
			var r = re(1);
			Assert.Equal("123", r.ToString(123));
		}
		[Fact]
		public void ByLazy()
		{
			var sc = new ServiceCollection();
			sc.AddTransient<IInterface<int>, Implement<int>>();
			sc.AddMicrosoftMemoryCacheAsLocalCache();
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
			sc.AddMicrosoftMemoryCacheAsLocalCache();
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
			sc.AddManagedService();
			sc.AddMicrosoftMemoryCacheAsLocalCache();
			sc.UseMemoryManagedServiceSource();
			sc.AddNewtonsoftJson();

			var sp = sc.BuildServiceResolver();

			var cfgs = sp.Resolve<MemoryServiceSource>();

			cfgs.SetConfig<IInterface<int>, Implement<int>>(1, new { },3);
			cfgs.SetConfig<IInterface<int>, Implement<int>>(2, new { },2,1);
			cfgs.SetConfig<IInterface<int>, Implement<int>>(3, new { },1,1);


			var re = sp.GetServices<IInterface<int>>(1);
			Assert.Equal(2, re.Count());
			foreach (var i in re)
				Assert.Equal("123", i.ToString(123));

		}
		[Fact]
		public void ManagedNamedEnumerable()
		{
			var sc = new ServiceCollection();
			sc.AddTransient<IInterface<int>, Implement<int>>();
			sc.AddManagedService();
			sc.AddMicrosoftMemoryCacheAsLocalCache();
			sc.UseMemoryManagedServiceSource();
			sc.AddNewtonsoftJson();

			var sp = sc.BuildServiceResolver();

			var cfgs = sp.Resolve<MemoryServiceSource>();

			cfgs.SetConfig<IInterface<int>, Implement<int>>(1, new { }, 3);
			cfgs.SetConfig<IInterface<int>, Implement<int>>(2, new { }, 2, 1,"svc1");
			cfgs.SetConfig<IInterface<int>, Implement<int>>(3, new { }, 1, 1,"svc2");
			cfgs.SetConfig<IInterface<int>, Implement<int>>(4, new { }, 0, 1, "svc2");

			var re = sp.GetServices<IInterface<int>>(1);
			Assert.Equal(3, re.Count());
			foreach (var i in re)
				Assert.Equal("123", i.ToString(123));

			re = sp.GetServices<IInterface<int>>(1,"svc2");
			Assert.Equal(2, re.Count());
			foreach (var i in re)
				Assert.Equal("123", i.ToString(123));

		}
		[Fact]
		public void ManagedNamedService()
		{
			var sc = new ServiceCollection();
			sc.AddTransient<IInterface<int>, Implement<int>>();
			sc.AddTransient<IInterface<int>, ImplementWithArg<int>>();
			sc.AddManagedService();
			sc.AddMicrosoftMemoryCacheAsLocalCache();
			sc.UseMemoryManagedServiceSource();
			sc.AddNewtonsoftJson();

			var sp = sc.BuildServiceResolver();

			var cfgs = sp.Resolve<MemoryServiceSource>();

			cfgs.SetConfig<IInterface<int>, Implement<int>>(1, new { }, 3);
			cfgs.SetConfig<IInterface<int>, Implement<int>>(2, new { }, 2, 1, "svc1");
			cfgs.SetConfig<IInterface<int>, Implement<int>>(3, new { }, 1, 1, "svc2");
			cfgs.SetConfig<IInterface<int>, ImplementWithArg<int>>(4, new {Prefix="svc4" }, 0, 1, "svc2");

			var re = sp.ResolveInternal<IInterface<int>>(1,"svc2");
			Assert.Equal("svc4123", re.ToString(123));
		}
		[Fact]
		public void InternalServiceExists()
		{
			var sc = new ServiceCollection();
			sc.AddTransient<IInterface<int>, Implement<int>>();
			sc.AddTransient<IInterface<int>, ImplementWithArg<int>>();
			sc.AddManagedService();
			sc.AddMicrosoftMemoryCacheAsLocalCache();
			sc.UseMemoryManagedServiceSource();
			sc.AddNewtonsoftJson();

			var sp = sc.BuildServiceResolver();

			var cfgs = sp.Resolve<MemoryServiceSource>();

			cfgs.SetConfig<IInterface<int>, ImplementWithArg<int>>(1, new { Prefix = "s1" }, 3);
			cfgs.SetConfig<IInterface<int>, ImplementWithArg<int>>(2, new { Prefix="s2" }, 2, 1);
			cfgs.SetConfig<IInterface<int>, ImplementWithArg<int>>(3, new { Prefix = "s3" }, 0, 1);

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
			sc.AddManagedService();
			sc.AddMicrosoftMemoryCacheAsLocalCache();
			sc.UseMemoryManagedServiceSource();
			sc.AddNewtonsoftJson();

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
			sc.AddMicrosoftMemoryCacheAsLocalCache();
			sc.UseMemoryManagedServiceSource();
			sc.AddNewtonsoftJson();

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
			sc.AddManagedService();
			sc.AddMicrosoftMemoryCacheAsLocalCache();
			sc.UseMemoryManagedServiceSource();
			sc.AddNewtonsoftJson();

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
			sc.AddManagedService();
			sc.AddMicrosoftMemoryCacheAsLocalCache();
			sc.UseMemoryManagedServiceSource();
			sc.AddNewtonsoftJson();

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
