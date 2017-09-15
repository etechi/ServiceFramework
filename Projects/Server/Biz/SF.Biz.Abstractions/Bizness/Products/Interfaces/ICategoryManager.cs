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
        [EntityIdent(typeof(SF.Auth.Identities.IIdentityManagementService))]
        [Ignore]
        public long? SellerId { get; set; }

        [Display(Name = "父目录")]
        [EntityIdent(typeof(ICategoryManager<>))]
        public long? ParentId { get; set; }

        [Display(Name = "对象状态")]
        public EntityLogicState? ObjectState { get; set; }
    }

	public interface ICategoryManager : ICategoryManager<CategoryEditable>
	{ }

	public interface ICategoryManager<TEditable> :
		IEntityManager<long, TEditable>,
		IEntitySource<long,TEditable, CategoryQueryArgument>
		where TEditable : CategoryEditable
	{
		Task<TEditable[]> BatchUpdate(long SellerId, TEditable[] Items);
		Task UpdateItems(long CategoryId, long[] Items);
        Task<long[]> LoadItems(long CategoryId);
	}
}
