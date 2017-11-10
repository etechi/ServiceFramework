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
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using System.Threading.Tasks;
using SF.Core.Events;

namespace SF.Entities
{
	public abstract class EntityEvent : CommonEvent
	{
		public abstract Type EntityType { get; }
		public Data.PostActionType PostActionType { get; set; }
		public DataActionType Action { get; set; }
		public string EntityIdent { get; set; }
		public Exception Exception { get; set; }

		object _CachedEntityInstance;
		public object GetCachedEntityInstance() => _CachedEntityInstance;
		public void SetCachedEntityInstance(object Instance) {
			if (Instance != null && !EntityType.IsAssignableFrom(Instance.GetType()))
				throw new ArgumentException($"类型错误,实体事件定义类型为:{EntityType},实际提供为:{Instance.GetType()}");
			_CachedEntityInstance = Instance;
		}
		public EntityEvent() { }
		public EntityEvent(object CachedEntity)
		{
			SetCachedEntityInstance(CachedEntity);
		}
	}
	public class EntityChanged<TEntity> : EntityEvent
	{
		public EntityChanged()
		{
		}

		public EntityChanged(TEntity Entity) : base(Entity)
		{
		}

		public override Type EntityType => typeof(TEntity);
		
		public TEntity GetEntity()=>(TEntity)GetCachedEntityInstance();
		public void SetEntity(TEntity Entity) => SetCachedEntityInstance(Entity);
	}
	//public abstract class EntityRelationEvent : EntityEvent
	//{
	//	public abstract Type RelatedEntityType { get; }
	//}
	//public class EntityRelationChanged<TEntity, TRelatedEntityType> : EntityRelationEvent
	//{
	//	public override Type EntityType => typeof(TEntity);
	//	public override Type RelatedEntityType => typeof(TEntity);
	//	public TEntity Entity { get; set; }
	//	public TRelatedEntityType RelatedEntity { get; set; }
	//}
}
