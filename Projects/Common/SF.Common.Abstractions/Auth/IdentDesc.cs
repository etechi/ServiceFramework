using SF.Data;

namespace SF.Auth
{
	public class UserDesc : IObjectWithId<long>
    {
		public long Id { get; set; }
		public string NickName { get; set; }
		public string Icon { get; set; }
		public string Image { get; set; }
	}
}

