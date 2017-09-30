using SF.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Products
{
    public class ProductTypeQueryArgument : IQueryArgument<ObjectKey<long>>
    {
		public ObjectKey<long> Id { get; set; }

        [Display(Name = "对象状态")]
        public EntityLogicState? ObjectState { get; set; }

		[Display(Name = "类型名称")]
		public string Name { get; set; }
    }
	public interface IProductTypeManager: IProductTypeManager<ProductTypeInternal, ProductTypeEditable>
	{

	}

	public interface IProductTypeManager<TInternal, TEditable> :
		IEntityManager<ObjectKey<long>, TEditable>,
		IEntitySource<ObjectKey<long>, TInternal,ProductTypeQueryArgument>
		where TInternal : ProductTypeInternal
		where TEditable : ProductTypeEditable
	{
	}
}
