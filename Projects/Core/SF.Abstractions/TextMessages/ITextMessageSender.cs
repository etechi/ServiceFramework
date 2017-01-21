using System.Threading.Tasks;

namespace SF.TextMessages
{
	public class TextMessage
	{
		public string Title { get; set; }
		//public string 
	}
	public interface ITextMessageSender
	{
		Task<long> Send(long? DestUserId, string Dest, string Title);
	}

}
