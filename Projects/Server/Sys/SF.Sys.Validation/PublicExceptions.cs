#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys
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
	public class PublicNotSigninException : PublicDeniedException
	{
		public PublicNotSigninException(string message="未登录") : base(message) { }
		public PublicNotSigninException(string message, System.Exception innerException) : base(message, innerException) { }
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
