using System;
using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using SF.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SF.UT.Data
{
	public class User
	{
		public string Id { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

	}
	public class UserEditable
	{
		public string Id { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public List<Post> Posts { get; set; }
	}
	public class Post
	{
		public string Id { get; set; }

		public string UserId { get; set; }

		public User User { get; set; }

		public string Content { get; set; }
	}
}
