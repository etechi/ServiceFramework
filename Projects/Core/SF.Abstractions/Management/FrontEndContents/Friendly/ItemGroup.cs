using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndContents.Friendly
{
	public class ItemGroup<T> : 
		IEntityWithId<long> 
		where T:LinkItemBase 
	{
        [Key]
        [Ignore]
        public long Id { get; set; } = 1;

		[Display(Name = "名称")]
		[TableVisible]
		[ReadOnly(true)]
		[Ignore]
		public string Name { get; set; }

		[Display(Name = "帮助")]
		[TableVisible]
		[ReadOnly(true)]
		[Ignore]
		public string Help { get; set; }

		[TableRows]
        public T[] Items { get; set; }
	}
}
