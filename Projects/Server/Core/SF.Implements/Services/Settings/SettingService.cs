using System;

using System.Collections.Generic;
using SF.Core.ServiceManagement.Internals;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SF.Services.Settings
{
	public class SettingService<T> : ISettingService<T>
	{
		public T Value { get; }
		public SettingService(T Value)
		{
			this.Value = Value;
		}
	}
}
