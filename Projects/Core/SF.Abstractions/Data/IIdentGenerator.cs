﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data
{
	public interface IIdentGenerator
	{
        Task<long> Generate(string Type);
	}
}
