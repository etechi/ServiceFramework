﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.CallGuarantors
{
	
	public static class ICallGuarantorExtension
	{
		public static Task DelayCall(
			this ICallGuarantor CallGuarantor,
			string CallableName,
			string CallContext,
			string CallArgument,
			Exception Exception,
			string Title,
			DateTime CallTime,
			int ExpireSeconds=365*86400,
			int DelaySecondsOnError=5*60
			)
		{
			return CallGuarantor.Schedule(
				CallableName,
				CallContext,
				CallArgument,
				Exception,
				Title,
				CallTime,
				ExpireSeconds,
				DelaySecondsOnError
				);
		}

		public static Task Call(
			this ICallGuarantor CallGuarantor,
			string CallableName,
			string CallContext,
			string Argument,
			Exception Exception,
			string Title,
			int ExpireSeconds,
			int DelaySecondsOnError
			)
		{
			return CallGuarantor.Schedule(
				CallableName,
				CallContext,
				Argument,
				Exception,
				Title,
				DateTime.MinValue,
				ExpireSeconds,
				DelaySecondsOnError
				);
		}
	}
}
