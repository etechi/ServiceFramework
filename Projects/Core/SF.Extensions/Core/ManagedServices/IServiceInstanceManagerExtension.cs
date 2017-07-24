using System;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using SF.Data.Entity;
namespace SF.Core.ServiceManagement.Management
{
	public static class IServiceInstanceManagerExtension
	{
		public static async Task<long> ResolveDefaultService<I>(
			this IServiceInstanceManager Manager
			)
		{
			return await Manager.ResolveEntity(
					new ServiceInstanceQueryArgument
					{
						ServiceType = typeof(I).FullName,
						IsDefaultService = true
					});
		}

		static void UpdateServiceModel<I,T>(Models.ServiceInstanceEditable e,
			object Setting,
			long? ParentId=null,
			string Name = null,
			string Title = null,
			string Description = null,
			string ServiceIdent = null
			)
		{
			var comment = typeof(T).Comment();
			e.ImplementType = typeof(T).FullName + "@" + typeof(I).FullName;
			e.ServiceType = typeof(I).FullName;
			e.Priority = 0;
			e.ObjectState = Data.LogicObjectState.Enabled;
			e.ServiceIdent = ServiceIdent;
			//e.SettingType = typeof(T).FullName + "CreateArguments";
			e.Name = Name ?? comment.Name;
			e.Title = Title ?? comment.Name;
			e.ParentId = ParentId;
			if (e.DisplayData == null) e.DisplayData = new Data.Models.UIDisplayData();
			e.DisplayData.Description = Description ?? comment.Description;
			e.Setting = Json.Stringify(Setting);
		}
		public static async Task<Models.ServiceInstanceEditable> EnsureDefaultService<I,T>(
			this IServiceInstanceManager Manager,
			object Setting,
			long? ParentId=null,
			string Name=null,
			string Title=null,
			string Description=null,
			string ServiceIdent=null
			)
		{
			return await Manager.EnsureEntity(
				await Manager.ResolveEntity(
					new ServiceInstanceQueryArgument
					{
						ServiceType = typeof(I).FullName,
						IsDefaultService = true
					}),
				() => new Models.ServiceInstanceEditable { },
				e => UpdateServiceModel<I, T>(e, Setting, ParentId,Name, Title, Description, ServiceIdent)
				);
	
		}
		public static async Task<Models.ServiceInstanceEditable> TryAddService<I, T>(
			this IServiceInstanceManager Manager,
			object Setting,
			long? ParentId = null,
			string Name = null,
			string Title = null,
			string Description = null,
			string ServiceIdent = null
			)
		{
			var re=await Manager.QueryAsync(
				new ServiceInstanceQueryArgument
				{
					ServiceType = typeof(I).FullName,
					ImplementId = typeof(T).FullName + "@" + typeof(I).FullName
				}, Data.Paging.Default
				);
			var items = re.Items.ToArray();
			if (items.Length > 1)
				return null;
			return await Manager.EnsureEntity(
				items.Length==0?0:items[0].Id,
				() => new Models.ServiceInstanceEditable { },
				e => UpdateServiceModel<I, T>(e, Setting, ParentId, Name, Title, Description, ServiceIdent)
				);

		}

		public static async Task SetServiceParent(
			this IServiceInstanceManager Manager,
			long ServiceId,
			long? ParentServiceId
			)
		{
			var re = await Manager.QueryAsync(
				new ServiceInstanceQueryArgument
				{
					Id=ServiceId,
				}, Data.Paging.Default
				);
			var items = re.Items.ToArray();
			if (items.Length > 1)
				throw new InvalidOperationException("找到多个服务实例");
			await Manager.EnsureEntity(
				items.Length == 0 ? 0 : items[0].Id,
				() => throw new InvalidOperationException("找不到服务实例：" + ServiceId),
				e => e.ParentId=ParentServiceId
				);

		}
	}
}
