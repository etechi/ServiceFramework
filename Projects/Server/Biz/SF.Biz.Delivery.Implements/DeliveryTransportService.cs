using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;
using ServiceProtocol.Data.Entity;
namespace ServiceProtocol.Biz.Delivery.Entity
{
    [DataObjectLoader("发货渠道")]
	public class DeliveryTransportService<TTransport,TModel> :
		ServiceProtocol.ObjectManager.EntityServiceObjectManager<int,TTransport, TTransport, TModel>,
		IDeliveryTransportService<TTransport>,
        IDataObjectLoader
		where TTransport:Transport,new()
		where TModel:Models.DeliveryTransport,new()
	{
		public DeliveryTransportService(IDataContext Context,Lazy<IModifyFilter> ModifyFilter) :base(Context, ModifyFilter)
		{
		}

        protected override Task<TTransport> MapModelToEditable(IContextQueryable<TModel> Query)
		{
			return Query.Select(m => new TTransport
			{
				Id=m.Id,
				ContactName = m.ContactName,
				ContactPhone = m.ContactPhone,

				Ident = m.Ident,
				Name = m.Name
			}).SingleOrDefaultAsync();
		}

        protected override IContextQueryable<TTransport> MapModelToInternal(IContextQueryable<TModel> Query)
		{
			return from m in Query
				   select new TTransport
				   {
					   Id=m.Id,
					   ContactName = m.ContactName,
						ContactPhone = m.ContactPhone,

						Ident = m.Ident,
						Name = m.Name
					};
		}

		protected override Task OnUpdateModel(ModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;
			Model.ContactName = obj.ContactName;
			Model.ContactPhone = obj.ContactPhone;
			Model.Ident = obj.Ident;
			Model.Name = obj.Name;
			return Task.CompletedTask;
		}

		public async Task<TTransport[]> List()
		{
			return await MapModelToInternal(Context.ReadOnly<TModel>()).ToArrayAsync();
		}

        async Task<IDataObject[]> IDataObjectLoader.Load(string Type, string[][] Keys)
        {
            return await DataObjectLoader.Load(
               Keys,
               id => int.Parse(id[0]),
               id => FindByIdAsync(id),
               async (ids) => {
                   var tmps = await MapModelToInternal(
                       Context.ReadOnly<TModel>().Where(a => ids.Contains(a.Id))
                       ).ToArrayAsync();
                   return await OnPrepareInternals(tmps);
               }
               );
        }
    }
}
