using System;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using System.Threading.Tasks;

namespace SF.Entities
{
	
	public class EntityModified<TEntity>
	{
		

		public DataActionType Action { get; set; }
		public long? ServiceId { get; set; }
		public TEntity Entity { get; set; }
		public DateTime Time { get; set; }
		public Data.PostActionType PostActionType { get; set; }

		
	}
}
