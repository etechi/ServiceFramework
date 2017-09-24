using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using Xunit;
using SF.Applications;
using SF.Core.Hosting;
using SF.Core.ServiceManagement;
using SF.Core.ServiceFeatures;
using System.Threading.Tasks;

namespace SF.Entities.Smart.Test
{
	public class Category
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public IEnumerable<Document> Documents { get; set; }
		public Category Parent { get; set; }
		public IEnumerable<Category> Children { get; set; }
		public DateTime CreatedTime { get; set; }
		public DateTime UpdatedTime { get; set; }
		public EntityLogicState LogicState { get; set; }

	}


}
