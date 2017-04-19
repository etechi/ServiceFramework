using SF.Data;

namespace SF.System.Auth
{
	public class UserDesc : IObjectWithId<long>
    {
		public int ScopeId { get; set; }
		public long Id { get; set; }
		public string NickName { get; set; }
		public string Icon { get; set; }
		public string Image { get; set; }
	}
}

