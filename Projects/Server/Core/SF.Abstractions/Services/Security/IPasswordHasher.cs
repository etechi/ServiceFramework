namespace SF.Services.Security
{
	public interface IPasswordHasher
	{
		string Hash(string Value,byte[] SecurityStamp);
	}
}

