﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Metadata
{
	[AttributeUsage(AttributeTargets.Property)]
	public class PasswordAttribute : MediaAttribute
	{
		public PasswordAttribute()
		{

		}
	}
}