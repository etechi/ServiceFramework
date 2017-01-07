using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using SF.Annotations;
using SF.Reflection;
using SF.Services.Metadata.Models;

namespace SF.Services.Metadata
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
	}

}
