using SF.Data.Models;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Common.Documents
{
	[EntityObject(nameof(Document))]
	public class Document : UIItemEntityBase<Category>
	{
		public string Content { get; set; }
	}
	[EntityObject(nameof(Category))]
	public class Category : UITreeContainerEntityBase<Category,Document>
	{
	}
}
