﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth
{
	public interface IPermission
	{
		string OperationId { get;}
		string ResourceId { get; }
	}
}
