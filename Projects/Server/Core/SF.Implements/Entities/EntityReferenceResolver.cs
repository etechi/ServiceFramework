using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Core.ServiceManagement;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Entities
{
	public class EntityReferenceResolver : IEntityReferenceResolver
	{
		
		class EntityReference<TKey,TEntity> : 
			IEntityReference
			where TEntity:IEntityWithName
		{
			public TEntity Instance { get; }
			public long ServiceId { get; }
			public EntityReferenceLoader Loader { get; }
			public EntityReference(TEntity Instance, long ServiceId, EntityReferenceLoader Loader)
			{
				this.ServiceId = ServiceId;
				this.Instance = Instance;
				this.Loader = Loader;
			}
			public string Name { get => Instance.Name; set => Instance.Name = value; }

			public string Id => ServiceEntityIdent.Create(
				ServiceId, 
				Entity<TEntity>.GetKey<TKey>(Instance).ToString()
				);

			public Task<object> Resolve()
			{
				return Task.FromResult((object)Instance);
			}
		}

		abstract class EntityReferenceLoader
		{
			public abstract Task<IEntityReference[]> LoadByType(IServiceProvider sp, long? ScopeId, IEnumerable<string> Idents);
			public abstract Task<IEntityReference[]> LoadByServiceInstance(IServiceProvider sp, long ServiceId, IEnumerable<string> Idents);
		}
		class EntityReferenceLoader<TKey, TEntity, TManager> :
			EntityReferenceLoader
			where TManager : class,IEntityLoadable<TKey,TEntity>, IManagedServiceWithId
			where TEntity:IEntityWithName
		{
			async Task<IEntityReference[]> Load(TManager manager, IEnumerable<string> Ids)
			{
				var Idents = Ids?.ToArray();
				if ((Idents?.Length ?? 0) == 0)
					return Array.Empty<IEntityReference>();
				if (Idents.Length == 1)
				{
					var ins = await manager.GetAsync((TKey)Convert.ChangeType(Idents[0], typeof(TKey)));
					return new[] { new EntityReference<TKey,TEntity>(ins, manager.ServiceInstanceId, this) };
				}
				else
				{
					var mel = manager as IEntityBatchLoadable<TKey, TEntity>;
					if (mel != null)
						return (await mel.GetAsync(
							Idents.Select(id => (TKey)Convert.ChangeType(id, typeof(TKey))).ToArray()
							))
							.Select(ins => new EntityReference<TKey, TEntity>(ins, manager.ServiceInstanceId, this))
							.ToArray();

					var re = new List<IEntityReference>();
					var el = (IEntityLoadable<TKey, TEntity>)manager;
					foreach (var id in Idents.Select(id => (TKey)Convert.ChangeType(id, typeof(TKey))))
					{
						var ins = await el.GetAsync(id);
						if (ins != null)
							re.Add(new EntityReference<TKey, TEntity>(ins,manager.ServiceInstanceId, this));
					}
					return re.ToArray();
				}
			}
			public override async Task<IEntityReference[]> LoadByType(IServiceProvider sp, long? ScopeId, IEnumerable<string> Idents)
			{
				var manager = sp.Resolve<TManager>(null, ScopeId);
				return await Load(manager, Idents);
			}
			public override async Task<IEntityReference[]> LoadByServiceInstance(IServiceProvider sp, long ServiceId, IEnumerable<string> Idents)
			{
				var manager = sp.Resolve<TManager>(ServiceId);
				return await Load(manager, Idents);
			}
		}

		IServiceProvider ServiceProvider { get; }
		IEntityMetadataCollection MetadataCollection { get; }
		System.Collections.Concurrent.ConcurrentDictionary<Type, EntityReferenceLoader> Loaders { get; } = new System.Collections.Concurrent.ConcurrentDictionary<Type, EntityReferenceLoader>();

		EntityReferenceLoader GetLoader(IEntityMetadata meta)
		{
			if (Loaders.TryGetValue(meta.EntityManagerType, out var l))
				return l;
			l = (EntityReferenceLoader)Activator.CreateInstance(
				typeof(EntityReferenceLoader<,,>)
				.MakeGenericType(
					meta.EntityKeyType, 
					meta.EntityDetailType, 
					meta.EntityManagerType
					));
			return Loaders.GetOrAdd(meta.EntityManagerType, l);
		}
		public EntityReferenceResolver(IServiceProvider ServiceProvider, IEntityMetadataCollection MetadataCollection)
		{
			this.ServiceProvider = ServiceProvider;
			this.MetadataCollection= MetadataCollection;
		}
		public async Task<IEntityReference[]> Resolve(long? ScopeId,string Type, IEnumerable<string> Keys)
		{
			var meta=MetadataCollection.FindByTypeIdent(Type);
			if (meta == null)
				return Array.Empty<IEntityReference>();
			var loader = GetLoader(meta);
			return await loader.LoadByType(ServiceProvider, ScopeId, Keys);
		}
		public async Task<IEntityReference[]> Resolve(long ServiceId, IEnumerable<string> Keys)
		{
			var svc = ServiceProvider.Resolver().ResolveDescriptorByIdent(ServiceId);
			var meta = MetadataCollection.FindByManagerType(svc.ServiceDeclaration.ServiceType);
			if (meta == null)
				return Array.Empty<IEntityReference>();
			var loader = GetLoader(meta);
			return await loader.LoadByServiceInstance(ServiceProvider, ServiceId, Keys);
		}
	}
}
