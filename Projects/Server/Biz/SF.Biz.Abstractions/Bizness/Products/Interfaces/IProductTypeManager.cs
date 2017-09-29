using SF.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Products
{
    public class ProductTypeQueryArgument : IQueryArgument<long>
    {
		public Option<long> Id { get; set; }

        [Display(Name = "对象状态")]
        public EntityLogicState? ObjectState { get; set; }

		[Display(Name = "类型名称")]
		public string Name { get; set; }
    }
	public interface IProductTypeManager: IProductTypeManager<ProductTypeInternal, ProductTypeEditable>
	{

	}

	public interface IProductTypeManager<TInternal, TEditable> :
		IEntityManager<TEditable>,
		IEntitySource<TInternal,ProductTypeQueryArgument>
		where TInternal : ProductTypeInternal
		where TEditable : ProductTypeEditable
	{
	}
}
