using SF.Data;
using SF.KB;

namespace SF.System.Auth.Identity.Models
{
	public class IdentDesc : IObjectWithId<long>
    {
		public long Id { get; set; }
		public string NickName { get; set; }
		public string Icon { get; set; }
		public string Image { get; set; }
	}
}

