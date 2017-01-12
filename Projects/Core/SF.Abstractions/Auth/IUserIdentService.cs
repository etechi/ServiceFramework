
namespace SF.Auth
{
    public class UserIdent
    {
        public int IdentServiceId { get; set; }
        public string IdentServiceName { get; set; }
        public string Value { get; set; }
        public string ExtraData { get; set; }
        public bool Validated { get; set; }
    }
    public interface IUserIdentService
    {

		Task BindUserIdent(byte IdentType, string Ident, TKey UserId, string ExtraData);
		Task UnbindUserIdent(byte IdentType, string Ident, TKey UserId);
		Task<KeyValuePair<byte, string>[]> QueryUserIdents(TKey UserId);

		Task<Tuple<int, string>> GetSignupIdent(IAuthUser<TKey> User);
		Task<TKey> FindUserIdBySignupIdent(byte IdentType, string Ident);

		Task<TKey> FindUserIdByIdent(byte IdentType, string Ident);
		Task<TKey> FindUserIdByExtIdent(string Provider, string ExtIdent);
		Task<string> GetUserExtIdent(TKey UserId, string Provider);

		Task BindUserExtIdent(string Provider, string ExtIdent, int IdentType, string Ident, TKey UserId);
		Task UnbindUserExtIdent(string Provider, string ExtIdent);

	}
}
