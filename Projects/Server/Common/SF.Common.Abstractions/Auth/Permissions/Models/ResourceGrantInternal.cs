using SF.Data.Models;
using SF.Entities;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Permissions.Models
{

	[EntityObject]
	public class ResourceGrantInternal : IEntityWithId<string>
	{
		[Comment(Name = "资源ID")]
		[ReadOnly(true)]
		[Layout(10)]
		public string Id { get; set; }

		[Display(Name = "资源分组")]
        [ReadOnly(true)]
        [Layout(15)]
        public string Group { get; set; }

        [Comment(Name = "资源项目")]
        [ReadOnly(true)]
        [Layout(20)]
        public string Name { get; set; }

        [Display(Name = "授权")]
        [Layout(25)]
        [EntityIdent(typeof(OperationInternal),ScopeField=nameof(Id))]
        public string[] OperationIds { get; set; }


    }
}
