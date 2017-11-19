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

using System.Threading.Tasks;
using System;
using SF.Core.ServiceManagement;
using System.Linq;
using SF.Data;
using System.Reflection;
using System.Linq.Expressions;
using System.Data.Common;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Core;

namespace SF.Entities
{
	public static class EntityEventExtension
	{
		public static async Task<E> LoadEntity<K,E>(this EntityChanged<E> e,Func<K,Task<E>> Loader)
		{
			//if (typeof(E).IsAssignableFrom(e.EntityType))
			//	throw new ArgumentException($"指定实体类型{typeof(E)}与事件实体类型不匹配{e.EntityType}");
			var i = e.GetCachedEntity();
			if (i != null)
				return (E)i;
			var key = Json.Parse<K>(e.Ident);
			var r =await Loader(key);
			if (r.IsDefault()) return default(E);
			e.SetCachedEntity(r);
			return (E)r;
		}
		public static Task<E> LoadEntity<K, E>(this EntityChanged<E> e, IEntityEditableLoader<K, E> Loader)
			=> e.LoadEntity<K,E>(k => Loader.LoadForEdit(k));

		public static Task<E> LoadEntity<K, E>(this EntityChanged<E> e, IServiceProvider Services)
			=> e.LoadEntity<K, E>(k => Services.Resolve< IEntityEditableLoader < K, E >>().LoadForEdit(k));

	}
}
