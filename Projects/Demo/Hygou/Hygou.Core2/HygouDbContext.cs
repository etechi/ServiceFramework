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

namespace Hygou
{
	public class HygouDbContext : DbContext
	{
		public HygouDbContext(DbContextOptions<HygouDbContext> options)
			: base(options)
		{ }

	}

}
