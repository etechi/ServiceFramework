﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.DI
{
	public interface IDIScope : 
		IDisposable
	{
		IServiceProvider ServiceProvider { get; }
	}
}