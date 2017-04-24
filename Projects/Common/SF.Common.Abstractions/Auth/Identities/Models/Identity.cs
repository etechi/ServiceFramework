using SF.Data;

namespace SF.Auth.Identities.Models
{
	public class Identity : IObjectWithId<long>
    {
		public string Entity { get; set; }
		public long Id { get; set; }
		public string Name { get; set; }
		public string Icon { get; set; }
	}
}

