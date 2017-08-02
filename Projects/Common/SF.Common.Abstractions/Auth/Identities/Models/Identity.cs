using SF.Data;
using SF.Metadata;

namespace SF.Auth.Identities.Models
{
	[EntityObject("身份标识")]
	[Comment("身份标识")]
	public class Identity : IObjectWithId<long>
    {
		public string Entity { get; set; }
		public long Id { get; set; }
		public string Name { get; set; }
		public string Icon { get; set; }
	}
}

