using ServiceProtocol.Annotations;
using ServiceProtocol.ObjectManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Biz.UIManager.Friendly
{
	public class ItemGroup<T> where T:LinkItemBase
	{
        [Key]
        [Ignore]
        public int Id { get; set; } = 1;

        [TableRows]
        public T[] Items { get; set; }
	}
}
