namespace SF.Auth.Users
{
	public interface IPasswordHasher
	{
		string Hash(string Value);
	}
}

