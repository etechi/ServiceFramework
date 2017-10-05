﻿using System;

namespace SF.Entities.AutoEntityProvider.Internals
{
	public class AutoEntityType
	{
		public AutoEntityType(string Namespace,Type Type, bool AutoGenerateDataModel)
		{
			this.Namespace = Namespace;
			this.Type = Type;
			this.AutoGenerateDataModel = AutoGenerateDataModel;
		}
		public string Namespace { get; }
		public bool AutoGenerateDataModel { get; }
		public Type Type { get;  }
	}
}
