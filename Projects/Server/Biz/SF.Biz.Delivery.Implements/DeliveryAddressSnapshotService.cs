using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;
using ServiceProtocol.Data.Entity;
namespace ServiceProtocol.Biz.Delivery.Entity
{
	public class DeliveryAddressSnapshotService<TModel,TLocation> :
		IDeliveryAddressSnapshotService
		where TModel: Models.DeliveryAddressSnapshot,new()
		where TLocation: Models.DeliveryLocation
	{
		public IDataContext Context { get; }
		public IDataContextFactory ContextFactory { get; }
		public DeliveryAddressSnapshotService(IDataContext Context, IDataContextFactory ContextFactory)
		{
			this.Context = Context;
			this.ContextFactory = ContextFactory;
		}
		
		protected virtual void OnCollectDeliveryAddressContent(
			StringBuilder builder, 
			DeliveryAddress Address,
			string LocationName,
			string ZipCode
			)
		{
			builder.AppendLine(Address.ContactName);
			builder.AppendLine(Address.ContactPhoneNumber);
			builder.AppendLine(Address.LocationId.ToString());
			builder.AppendLine(Address.Address);
			builder.AppendLine(LocationName);
			builder.AppendLine(ZipCode);
		}
		protected virtual void OnVerifyDeliveryAddress(DeliveryAddress Address)
		{
			Ensure.HasContent(Address.ContactName, "收件人");
			Ensure.HasContent(Address.ContactPhoneNumber, "联系电话");
			Ensure.HasContent(Address.Address, "地址");
			Ensure.NotDefault(Address.LocationId, "地区");

		}

		async Task<KeyValuePair<string, string>> FindLocationFullNameAndCode(int LocationId,string CurName,string Code)
		{
			var loc = await Context.ReadOnly<TLocation>().Where(l => l.Id == LocationId).SingleOrDefaultAsync();
			if (loc == null)
				throw new ArgumentException("地区ID不正确:" + LocationId);
			if ((loc.FullName != null || loc.Level==1) && (Code != null || loc.Code != null))
				return new KeyValuePair<string, string>((loc.FullName ??loc.Name) + CurName, Code ?? loc.Code);

			return await FindLocationFullNameAndCode(loc.ParentId.Value, loc.Name + CurName, loc.Code);
		}
		public async Task<int> GetAddressId(DeliveryAddress Address)
		{
			Ensure.NotNull(Address, nameof(Address));

			OnVerifyDeliveryAddress(Address);

			var loc = await FindLocationFullNameAndCode(Address.LocationId, "", null);
			if (string.IsNullOrEmpty(loc.Value))
				loc = new KeyValuePair<string, string>(loc.Key, Address.LocationId.ToString());


			var sb = new StringBuilder();
			OnCollectDeliveryAddressContent(sb, Address, loc.Key, loc.Value);
			var hash = sb.ToString().UTF8Bytes().MD5().Base64();

			var try_count = 5;
			for(;;)
			{
				var id = await Context.ReadOnly<TModel>().Where(a => a.Hash == hash).Select(a=>a.Id).SingleOrDefaultAsync();
				if (id!=0)
					return id;

				using (var ctx = ContextFactory.Create(null))
				{
					var m = new TModel
					{
						Hash = hash,
						Address = Address.Address,
						ContactName = Address.ContactName,
						ContactPhoneNumber = Address.ContactPhoneNumber,
						LocationId=Address.LocationId,
						LocationName= loc.Key,
						ZipCode= loc.Value
					};
					ctx.Add(m);
					try
					{
						await ctx.SaveChangesAsync();
						return m.Id;
					}
					catch (Exception)
					{
						try_count--;
						if (try_count <= 0)
							throw;
					}
				}
			}
		}

		public async Task<DeliveryAddressDetail> QueryAddress(int Id)
		{
			return await (from a in Context.ReadOnly<TModel>()
				where a.Id.Equals(Id)
				select new DeliveryAddressDetail
				{
					Address = a.Address,
					ContactName = a.ContactName,
					ContactPhoneNumber = a.ContactPhoneNumber,
					LocationId=a.LocationId,
					LocationName=a.LocationName,
					ZipCode=a.ZipCode
				}
				).SingleOrDefaultAsync();

		}
	}
}
