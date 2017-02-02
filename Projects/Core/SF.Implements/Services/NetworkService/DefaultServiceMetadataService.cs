﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using SF.Services.NetworkService.Metadata;

namespace SF.Services.NetworkService
{
	public class DefaultServiceMetadataService : IServiceMetadataService
	{
		Library Library { get; }
		public DefaultServiceMetadataService(Library lib)
		{
			this.Library = lib;
		}
		public Library Json()
		{
			return Library;
		}

		public string Typescript(bool all = true)
		{
			var tb = new TypeScriptProxyBuilder(
				(c, a) =>
				all ||
				a.GrantInfo==null
				);
			var code = tb.Build(Library);
			return code;
		}

	}

}
