using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF
{
	public class ServiceProtocolException : global::System.Exception
	{
		public ServiceProtocolException(string message) : base(message) { }
		public ServiceProtocolException(string message,System.Exception innerException) : base(message,innerException) { }
	}

	public class PublicException : ServiceProtocolException
	{
		public PublicException(string message) : base(message) { }
		public PublicException(string message, System.Exception innerException) : base(message, innerException) { }
	}
	public class PublicDeniedException : PublicException
	{
		public PublicDeniedException(string message) : base(message) { }
		public PublicDeniedException(string message, System.Exception innerException) : base(message, innerException) { }
	}
	public class UserInputException: PublicException
	{
		public UserInputException(string message) : base(message) { }
		public UserInputException(string message, System.Exception innerException) : base(message, innerException) { }
	}
	public class PublicNotSupportedException : PublicException
	{
		public PublicNotSupportedException(string message) : base(message) { }
		public PublicNotSupportedException(string message, System.Exception innerException) : base(message, innerException) { }
	}
	public class PublicArgumentException : PublicException
	{
		public PublicArgumentException(string message) : base(message) { }
		public PublicArgumentException(string message, System.Exception innerException) : base(message, innerException) { }
	}
	public class PublicInvalidOperationException : PublicException
	{
		public PublicInvalidOperationException(string message) : base(message) { }
		public PublicInvalidOperationException(string message, System.Exception innerException) : base(message, innerException) { }
	}
}
