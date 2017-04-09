namespace SF.Security
{
	public interface IPasswordHasher
	{
		string Hash(string Value);
	}
}

