using System;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;

namespace SF.Entities
{
	
	public class EntityModified<TKey,TModel>
	{
		public DataActionType Action { get; set; }
		public long? ServiceId { get; set; }
		public TKey Id { get; set; }
		public DateTime Time { get; set; }
		public Data.PostActionType PostActionType { get; set; }
	}
}
