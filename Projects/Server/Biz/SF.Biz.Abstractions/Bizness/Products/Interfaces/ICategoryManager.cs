using SF.Auth.Identities.Models;
using SF.Entities;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Products
{
    public class CategoryQueryArgument : IQueryArgument<long>
    {
		public Option<long> Id { get; set; }

        [Display(Name = "卖家")]
        [EntityIdent(typeof(Identity))]
        [Ignore]
        public long? SellerId { get; set; }

        [Display(Name = "父目录")]
        [EntityIdent(typeof(CategoryInternal))]
        public long? ParentId { get; set; }

        [Display(Name = "对象状态")]
        public EntityLogicState? ObjectState { get; set; }

		[Display(Name = "名称")]
		public string Name { get; set; }
	}

	public interface ICategoryManager : ICategoryManager<CategoryInternal>
	{ }

	public interface ICategoryManager<TEditable> :
		IEntityManager< TEditable>,
		IEntitySource<TEditable, CategoryQueryArgument>
		where TEditable : CategoryInternal
	{
		//Task<TEditable[]> BatchUpdate(long SellerId, TEditable[] Items);
		//Task UpdateItems(long CategoryId, long[] Items);
  //      Task<long[]> LoadItems(long CategoryId);
	}
}
