using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Data;
using SF.Entities;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

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

			IEntityServiceDeclarer Add<I, T>(string Ident, string Name, bool ManagedService,params Type[] EntityTypes)
				where I : class
				where T : I;

		}

		public class EntityServiceDeclarer : IEntityServiceDeclarer
		{
			public IServiceCollection Services { get; set; }
			public List<IEntityServiceDescriptor> Descriptors { get; } = new List<IEntityServiceDescriptor>();
			public IEntityServiceDeclarer Add<I, T>(string Ident,string Name,bool ManagedService,params Type[] EntityTypes)
				where I:class
				where T:I
			{
				if(ManagedService)
					Services.AddManagedScoped<I, T>(
						async (sp,svc)=>
						{
							if (svc is IEntityAllRemover ear)
								await ear.RemoveAllAsync();
						}
						);
				else
					Services.AddScoped<I, T>();

				var loadableType=typeof(I).AllInterfaces().First(i => i.IsGenericTypeOf(typeof(IEntityLoadable<,>)));
				if (loadableType != null)
					Services.Add(new ServiceDescriptor(loadableType, sp => sp.GetService(typeof(I)),ServiceImplementLifetime.Scoped));

				var batchLoadableType = typeof(I).AllInterfaces().First(i => i.IsGenericTypeOf(typeof(IEntityBatchLoadable<,>)));
				if (batchLoadableType != null)
					Services.Add(new ServiceDescriptor(batchLoadableType, sp => sp.GetService(typeof(I)), ServiceImplementLifetime.Scoped));

				
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
			return declarer.Add<I,T>(null, null, true, EntityTypes);
		}
		public static IEntityServiceDeclarer AddUnmanaged<I, T>(this IEntityServiceDeclarer declarer, params Type[] EntityTypes)
			where I : class
				where T : I
		{
			return declarer.Add<I, T>(null, null,false, EntityTypes);
		}

		public static IEntityServiceDeclarer Add<I, T>(this IEntityServiceDeclarer declarer, string Ident,string Name,params Type[] EntityTypes)
			where I : class
				where T : I
		{
			return declarer.Add<I, T>(Ident,Name, true, EntityTypes);
		}
		public static IEntityServiceDeclarer AddUnmanaged<I, T>(this IEntityServiceDeclarer declarer, string Ident, string Name, params Type[] EntityTypes)
			where I : class
				where T : I
		{
			return declarer.Add<I, T>(Ident, Name, false, EntityTypes);
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
				Services=sc
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