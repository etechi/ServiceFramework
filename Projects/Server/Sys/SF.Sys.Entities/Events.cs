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

using SF.Sys.Events;
using System;

namespace SF.Sys.Entities
{
	public class EntityChanged<TEntity> : IEvent
	{
		public Type EventType => typeof(TEntity);
		public string Target { get; set; }
		public Exception Exception { get; set; }
		public long? ServiceId { get; set; }

		public DateTime Time { get; set; }

		static string EntityTypeFullName { get; } = typeof(TEntity).FullName;
		public string Source => EntityTypeFullName;
		public string Type => "SF.Sys.Entities.EntityChanged";
		public DataActionType Action { get; set; }
		public string TraceIdent { get; set; }

		object _CachedEntity;
		public void SetCachedEntity(object Instance)
		{
			_CachedEntity = Instance;
		}
		public object GetCachedEntity() => _CachedEntity;

		public EntityChanged() { }
		public EntityChanged(object CachedEntity)
		{
			SetCachedEntity(CachedEntity);
		}
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
