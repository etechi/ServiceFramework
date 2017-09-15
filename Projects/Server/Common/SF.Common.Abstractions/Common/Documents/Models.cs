using SF.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Common.Documents
{
	public class Document : UITreeNodeEntityBase<Document>
	{
		public string Content { get; set; }
	}
	public class Category : UITreeContainerEntityBase<Category,Document>
	{
	}
}
