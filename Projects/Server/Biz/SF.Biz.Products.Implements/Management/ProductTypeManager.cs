#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Entities;
using SF.Sys.Data;
namespace SF.Biz.Products.Entity
{


	public class ProductTypeManager:
		ModidifiableEntityManager<ObjectKey<long>, ProductTypeInternal,ProductTypeQueryArgument,  ProductTypeEditable, DataModels.DataProductType>,
		IProductTypeManager
    {
		public ProductTypeManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
        protected override IQueryable<ProductTypeInternal> OnMapModelToDetail(IQueryable<DataModels.DataProductType> Query)
		{
			return from c in Query
				   select new ProductTypeInternal
				   {
					   Id = c.Id,
					   Name = c.Name,
					   CreatedTime = c.CreatedTime,
					   UpdatedTime = c.UpdatedTime,
					   Title = c.Title,
					   Unit = c.Unit,
					   LogicState = c.LogicState,
					   Icon = c.Icon,
					   Image = c.Image,
						ProductCount = c.ProductCount,
                       DeliveryProvider=c.DeliveryProvider
                   };
		}
        protected override Task<ProductTypeEditable> OnMapModelToEditable(IDataContext ctx,IQueryable<DataModels.DataProductType> Query)
		{
			return (from c in Query
					select new ProductTypeEditable
					{
						Id = c.Id,
						Name = c.Name,
						Title = c.Title,
						Order = c.Order,
						Unit = c.Unit,
						LogicState = c.LogicState,
						Icon = c.Icon,
						Image = c.Image,
						ProductCount = c.ProductCount,
                        DeliveryProvider=c.DeliveryProvider
                    }).SingleOrDefaultAsync();
		}
		
		protected override Task OnUpdateModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;
			Model.Name = obj.Name;
			Model.Title = obj.Title;
			Model.Icon = obj.Icon;
			Model.Image = obj.Image;
			Model.LogicState = obj.LogicState;
			Model.Unit = obj.Unit;
			Model.UpdatedTime = Now;
            Model.DeliveryProvider = obj.DeliveryProvider;

            return Task.CompletedTask;
		}
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var Model = ctx.Model;
			Model.Id = await IdentGenerator.GenerateAsync<DataModels.DataProductType>();
			Model.CreatedTime = Now;
		}
		protected override Task<DataModels.DataProductType> OnLoadModelForUpdate(ObjectKey<long> Id, IQueryable<DataModels.DataProductType> ctx)
		{
			return ctx
				.Where(s => s.Id == Id.Id)
				//.Include(s => s.Items)
				.SingleOrDefaultAsync();
		}
		protected override IQueryable<DataModels.DataProductType> OnBuildQuery(IQueryable<DataModels.DataProductType> Query, ProductTypeQueryArgument Arg)
		{
			return Query
				.Filter(Arg.LogicState, p => p.LogicState)
				.Filter(Arg.Name,p=>p.Name);
		}
		protected override PagingQueryBuilder<DataModels.DataProductType> PagingQueryBuilder =>new PagingQueryBuilder<DataModels.DataProductType>(
			"name",
			i => i
			.Add("name", c => c.Name)
			.Add("updated", c => c.UpdatedTime, true)
			.Add("created", c => c.CreatedTime, true)
			);
	

    }
}
