﻿#region Apache License Version 2.0
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys
{
	public class ExternalServiceException : Exception
	{
		public bool Retryable { get; set; }
		public ExternalServiceException(string message) : base(message) { }
		public ExternalServiceException(string message,Exception innerException) : base(message, innerException) { }
	}
	public static class ExceptionExtension
	{
		public static Exception GetRootException(this Exception e)
		{
			if (e == null)
				return null;
			while (e.InnerException != null)
				e = e.InnerException;
			return e;
		}
	}
}
