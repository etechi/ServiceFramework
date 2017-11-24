#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.UT.Data.DataModels
{
	using IndexAttribute = SF.Sys.Data.IndexAttribute;

	[ComplexType]
	public class Location
	{
		[MaxLength(100)]
		[Required]
		public string City { get; set; }

		[MaxLength(100)]
		[Required]
		public string Address { get; set; }
	}
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

		public Location Location { get; set; }
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
