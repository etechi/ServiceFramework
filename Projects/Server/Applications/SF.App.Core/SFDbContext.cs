using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SF.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace SF.Applications
{
	public class SFDbContext : DbContext
	{
		public SFDbContext(DbContextOptions<SFDbContext> options)
			: base(options)
		{ }

	}

}
