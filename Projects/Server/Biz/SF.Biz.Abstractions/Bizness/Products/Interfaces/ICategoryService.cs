using SF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Products
{
	
	public interface ICategoryService<TCategory>
	   where TCategory : CategoryCached
	{
		Task<QueryResult<TCategory>> GetChildCategories(long CategoryId,Paging paging);
		Task<TCategory> GetCategory(long CategoryId);
	}
}
