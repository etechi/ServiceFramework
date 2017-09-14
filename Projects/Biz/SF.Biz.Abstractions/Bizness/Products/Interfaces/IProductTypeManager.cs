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
    }

    public interface IProductTypeManager<TInternal, TEditable> :
		IEntityManager<long, TEditable>,
		IEntitySource<long, TInternal,ProductTypeQueryArgument>
		where TInternal : ProductTypeInternal
		where TEditable : ProductTypeEditable
	{
	}
}
