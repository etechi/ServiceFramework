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

using SF.Sys.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Sys.Entities
{
	public class EntityReferenceResolver : IEntityReferenceResolver
	{
		
		class EntityReference<TKey,TEntity> : 
			IEntityReference
			where TEntity:IEntityWithName
			where TKey:IEquatable<TKey>
		{
			public TEntity Instance { get; }
			public string Id { get; }
			public EntityReference(TEntity Instance, string Id)
			{
				this.Instance = Instance;
				this.Id = Id;
			}
			public string Name { get => Instance.Name; set => Instance.Name = value; }

			public Task<object> Resolve()
			{
				return Task.FromResult((object)Instance);
			}
		}

		abstract class EntityReferenceLoader
		{
			public abstract Task<IEntityReference[]> LoadByType(IServiceProvider sp, long? ScopeId, string EntityIdent,IEnumerable<string> Idents);
			public abstract Task<IEntityReference[]> LoadByServiceInstance(IServiceProvider sp, long ServiceId, IEnumerable<string> Idents);
		}
		class EntityReferenceLoader<TKey, TEntity, TManager> :
			EntityReferenceLoader
			where TManager : class,IEntityLoadable<ObjectKey<TKey>,TEntity>
			where TEntity:IEntityWithName
			where TKey:IEquatable<TKey>
		{

			EntityReference<TKey, TEntity> CreateReference(TEntity e, string EntityIdent, long? ServiceId)
			{
				var key = Entity<TEntity>.GetKey<ObjectKey<TKey>>(e).Id.ToString();
				var id=EntityIdent == null?
					"i-" + ServiceId.Value + "-" + key:
					EntityIdent + "-" + key;
				return new EntityReference<TKey, TEntity>(e, id);
			}
			async Task<IEntityReference[]> Load(TManager manager,string EntityIdent, long? ServiceId, IEnumerable<string> Ids)
			{
				var Idents = Ids?.ToArray();
				if ((Idents?.Length ?? 0) == 0)
					return Array.Empty<IEntityReference>();
				if (Idents.Length == 1)
				{
					var ins = await manager.GetAsync(new ObjectKey<TKey> { Id = (TKey)Convert.ChangeType(Idents[0], typeof(TKey)) });
					if (ins.IsDefault())
						return Array.Empty<IEntityReference>();
					return new[] {CreateReference(ins,EntityIdent,ServiceId)};
				}
				else
				{
					var mel = manager as IEntityBatchLoadable<ObjectKey<TKey>, TEntity>;
					if (mel != null)
						return (await mel.BatchGetAsync(
							Idents.Select(id =>new ObjectKey<TKey> { Id = (TKey)Convert.ChangeType(id, typeof(TKey)) }).ToArray()
							))
							.Select(ins => CreateReference(ins, EntityIdent,ServiceId))
							.ToArray();

					var re = new List<IEntityReference>();
					var el = (IEntityLoadable<ObjectKey<TKey>, TEntity>)manager;
					foreach (var id in Idents.Select(id => (TKey)Convert.ChangeType(id, typeof(TKey))))
					{
						var ins = await el.GetAsync(new ObjectKey<TKey> { Id = id });
						if (ins != null)
							re.Add(CreateReference(ins, EntityIdent, ServiceId));
					}
					return re.ToArray();
				}
			}
			public override async Task<IEntityReference[]> LoadByType(IServiceProvider sp, long? ScopeId, string EntityIdent, IEnumerable<string> Idents)
			{
				var manager = sp.Resolve<TManager>(null, ScopeId);
				return await Load(manager,EntityIdent,null,  Idents);
			}
			public override async Task<IEntityReference[]> LoadByServiceInstance(IServiceProvider sp, long ServiceId, IEnumerable<string> Idents)
			{
				var manager = sp.Resolve<TManager>(ServiceId);
				return await Load(manager, null,ServiceId,Idents);
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
					meta.EntityKeyType.GenericTypeArguments[0], 
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
			return await loader.LoadByType(ServiceProvider, ScopeId, meta.Ident,  Keys);
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

		public string FindEntityIdent(Type Type)
		{
			return MetadataCollection.FindByEntityType(Type)?.Ident;
		}
	}
}
