using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Annotations
{
	[AttributeUsage(AttributeTargets.Property)]
	public class TableRowsAttribute : Attribute
	{
		public string ColumnsPropertyName { get; set; }
		public string ColumnDescriptionPropertyName { get; set; }
		public string RowDescriptionPropertyName { get; set; }
		public string CellsPropertyName { get; set; }
	}
}
