using SF.Data.Models;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Common.Documents
{
	[EntityObject]
	public class Document : UITreeNodeEntityBase<Document>
	{
		public string Content { get; set; }
	}
	[EntityObject]
	public class Category : UITreeContainerEntityBase<Category,Document>
	{
	}
}
