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

using SF.Sys.Reflection;
using SF.Sys.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Sys.Entities
{	
    public class EntityInterfaceResolver<K, T> : IEntityInterfaceResolver<K, T>
        where K : IEquatable<K>
    {
        static ConcurrentDictionary<string, Loader> Loaders = new ConcurrentDictionary<string, Loader>();

        abstract class Loader
        {
            public abstract Task<Dictionary<K, T>> Load(IServiceProvider serviceProvider, K[] Ids);
            public abstract Task<T> Load(IServiceProvider serviceProvider, K Id);
        }
        class Loader<E, M> : Loader
            where M : class, IEntityBatchLoadable<ObjectKey<K>, E>, IEntityLoadable<ObjectKey<K>, E>
            where E : class, T,IEntityWithId<K>
        {
            public override async Task<Dictionary<K,T>> Load(IServiceProvider serviceProvider, K[] Ids)
            {
                var re = await serviceProvider.Resolve<M>().BatchGetAsync(ObjectKey.From(Ids), null);
                return re.Where(i => i != null).ToDictionary(i => i.Id, i => (T)i);
            }
            public override async Task<T> Load(IServiceProvider serviceProvider, K Id)
            {
                var re = await serviceProvider.Resolve<M>().GetAsync(Id);
                return re;
            }
        }
        IServiceProvider ServiceProvider { get; }
        public EntityInterfaceResolver(IServiceProvider ServiceProvider)
        {
            this.ServiceProvider = ServiceProvider;
        }

        Loader GetLoader(string EntityIdent)
        {
            if (!Loaders.TryGetValue(EntityIdent, out var loader))
            {
                var metas = ServiceProvider.Resolve<IEntityMetadataCollection>();
                var e = metas.FindByTypeIdent(EntityIdent);
                if (e == null)
                    throw new PublicArgumentException("找不到指定的实体类型:" + EntityIdent);
                if (!e.EntityDetailType.AllInterfaces().Any(i => i == typeof(T)))
                    throw new PublicInvalidOperationException($"实体类型{e.EntityDetailType}不会实现接口{typeof(T)}");
                loader = (Loader)Activator.CreateInstance(typeof(Loader<,>).MakeGenericType(e.EntityDetailType, e.EntityManagerType));
                loader = Loaders.GetOrAdd(EntityIdent, loader);
            }
            return loader;
        }
        public Task<Dictionary<K,T>> Resolve(string EntityIdent, K[] Ids)
        {
            var loader = GetLoader(EntityIdent);
            return loader.Load(ServiceProvider, Ids);
        }
        public Task<T> Resolve(string EntityIdent, K Id)
        {
            var loader = GetLoader(EntityIdent);
            return loader.Load(ServiceProvider, Id);
        }
    }
}
