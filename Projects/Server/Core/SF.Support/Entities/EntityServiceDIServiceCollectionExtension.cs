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
using System.Linq;
using System.Threading.Tasks;
using SF.Data;
using SF.Entities;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using SF.Auth.Permissions;

namespace SF.Core.ServiceManagement
{
	public static class EntityServiceDIServiceCollectionExtension
	{
		public class EntityServiceDescriptor : IEntityServiceDescriptor
		{
			public Type ServiceType { get; set; }
			public Type[] EntityTypes { get; set; }
			public string Ident { get; set; }
			public string Name { get; set; }

			IEnumerable<Type> IEntityServiceDescriptor.EntityTypes => EntityTypes;
		}
		public class EntityServiceDescriptorGroup : 
			IEntityServiceDescriptorGroup
		{
			public string Name { get; set; }
			public string Ident { get; set; }
			public IEntityServiceDescriptor[] Descriptors { get; set; }

			IEnumerator IEnumerable.GetEnumerator()
				=> Descriptors.GetEnumerator();

			IEnumerator<IEntityServiceDescriptor> IEnumerable<IEntityServiceDescriptor>.GetEnumerator()
				=> Descriptors.Cast<IEntityServiceDescriptor>().GetEnumerator();

		}
		public interface IEntityServiceDeclarer
		{

			IEntityServiceDeclarer Add<I, T>(string Ident, string Name, bool ManagedService,bool IsDataScope,params Type[] EntityTypes)
				where I : class
				where T : I;

		}

		public class EntityServiceDeclarer : IEntityServiceDeclarer
		{
			public IServiceCollection Services { get; set; }
			public List<IEntityServiceDescriptor> Descriptors { get; } = new List<IEntityServiceDescriptor>();
			public string GroupName { get; set; }
			static Type[] EntityManagerInterfaceTypes { get; } = new[]
			{
				typeof(IEntityLoadable<,>),
				typeof(IEntityBatchLoadable<,>),
				typeof(IEntityQueryable<,>),
				typeof(IEntityManager<,>)
			};

			public IEntityServiceDeclarer Add<I, T>(string Ident,string Name,bool ManagedService,bool IsDataScope,params Type[] EntityTypes)
				where I:class
				where T:I
			{
				if(ManagedService)
					Services.AddManagedTransient<I, T>(
						async (sp,svc)=>
						{
							if (svc is IEntityAllRemover ear)
								await ear.RemoveAllAsync();
						},
						IsDataScope: IsDataScope
						);
				else
					Services.AddTransient<I, T>();

				typeof(I).AllInterfaces()
					.Select(i => (svc: typeof(I), i: i))
					.Where(i => EntityManagerInterfaceTypes.Any(it => i.i.IsGenericTypeOf(it)))
					.ForEach(i =>
					{
						Services.Add(new ServiceDescriptor(i.i, sp => sp.GetService(i.svc), ServiceImplementLifetime.Transient));
					});
				Services.AddAuthResource(Ident, GroupName, Name, null, new[]{
					Operations.Read,
					Operations.Create,
					Operations.Update,
					Operations.Remove
				});
				Descriptors.Add(
					new EntityServiceDescriptor
					{
						Ident=Ident,
						Name=Name,
						ServiceType = typeof(I),
						EntityTypes = EntityTypes
					}
				);

				return this;
			}
		}
		public static IEntityServiceDeclarer Add<I, T>(this IEntityServiceDeclarer declarer, params Type[] EntityTypes)
			where I : class
				where T : I
		{
			return declarer.Add<I,T>(null, null, true, false,EntityTypes);
		}
		public static IEntityServiceDeclarer AddUnmanaged<I, T>(this IEntityServiceDeclarer declarer, params Type[] EntityTypes)
			where I : class
				where T : I
		{
			return declarer.Add<I, T>(null, null,false, false, EntityTypes);
		}

		public static IEntityServiceDeclarer Add<I, T>(this IEntityServiceDeclarer declarer, string Ident,string Name,bool IsDataScope,params Type[] EntityTypes)
			where I : class
				where T : I
		{
			return declarer.Add<I, T>(Ident,Name, true, IsDataScope,EntityTypes);
		}
		public static IEntityServiceDeclarer Add<I, T>(this IEntityServiceDeclarer declarer, string Ident, string Name, params Type[] EntityTypes)
			where I : class
				where T : I
		{
			return declarer.Add<I, T>(Ident, Name, true, false, EntityTypes);
		}
		public static IEntityServiceDeclarer AddUnmanaged<I, T>(this IEntityServiceDeclarer declarer, string Ident, string Name, params Type[] EntityTypes)
			where I : class
				where T : I
		{
			return declarer.Add<I, T>(Ident, Name, false, false,EntityTypes);
		}


		public static IServiceCollection EntityServices(
			this IServiceCollection sc,
			string GroupIdent,
			string GroupName,
			Func<IEntityServiceDeclarer, IEntityServiceDeclarer> Callback
			)
		{
			GroupIdent.IsNotNull(() => "实体组标识不能为空");
			GroupName.IsNotNull(() => "实体组名称不能为空");

			var declarer = new EntityServiceDeclarer()
			{
				Services=sc,
				GroupName=GroupName
			};
			Callback(declarer);

			if (declarer.Descriptors.Count(d => d.Name == null) > 1)
				throw new ArgumentException("只允许一个服务使用默认名称(默认名称为组名称)");

			if (declarer.Descriptors.Count(d => d.Ident == null) > 1)
				throw new ArgumentException("只允许一个服务使用默认标识(默认名称为组标识)");

			sc.AddSingleton<IEntityServiceDescriptorGroup>(new EntityServiceDescriptorGroup
			{
				Name = GroupName,
				Ident = GroupIdent,
				Descriptors = declarer.Descriptors.ToArray()
			});
			
			return sc;
		}
	}
   
}