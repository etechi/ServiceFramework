﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using SF.Core.ServiceManagement;

namespace SF.Data
{
	public class EntityModels
	{
		public string Prefix { get; }
		public Type[] Types { get; }
		public EntityModels(Type[] Types,string Prefix)
		{
			this.Types = Types;
			this.Prefix = Prefix; 
		}
	}
}