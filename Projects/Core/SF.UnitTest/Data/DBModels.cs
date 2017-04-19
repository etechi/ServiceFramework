using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data.Storage;

namespace SF.UT.Data.DataModels
{
	using IndexAttribute = SF.Data.Storage.IndexAttribute;
	[Table("User")]
	public class User
	{
		[Key]
		[Required]
		[StringLength(100)]
		public string Id { get; set; }

		[Required]
		[StringLength(100)]
		[Index]
		[Index("full",Order =1)]
		public string FirstName { get; set; }

		[StringLength(100)]
		[Index("full", Order = 2)]
		public string LastName { get; set; }

		[InverseProperty(nameof(Post.User))]
		public List<Post> Posts { get; set; }
	}
	[Table("Post")]
	public class Post
	{
		[Key]
		[Required]
		[StringLength(100)]
		public string Id { get; set; }

		[Index]
		[Required]
		[StringLength(100)]
		public string UserId { get; set; }

		[ForeignKey(nameof(UserId))]
		public User User { get; set; }

		public string Content { get; set; }
	}
}
