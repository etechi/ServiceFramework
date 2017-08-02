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
						ServiceType = typeof(I).GetFullName(),
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
			e.ImplementType = typeof(T).GetFullName() + "@" + typeof(I).GetFullName();
			e.ServiceType = typeof(I).GetFullName();
			e.Priority = 0;
			e.ObjectState = Data.LogicObjectState.Enabled;
			e.ServiceIdent = ServiceIdent;
			//e.SettingType = typeof(T).FullName + "CreateArguments";
			e.Name = Name ?? comment.Name;
			e.Title = Title ?? comment.Name;
			e.ParentId = ParentId;
			e.Description = Description ?? comment.Description;
			e.Setting = Json.Stringify(Setting);
		}
		public static async Task<long> TryGetDefaultService<I>(
			this IServiceInstanceManager Manager,
			long? ParentId = null
			)
			=> await Manager.ResolveEntity(
				new ServiceInstanceQueryArgument
				{
					ParentId = ParentId ?? 0,
					ServiceType = typeof(I).GetFullName(),
					IsDefaultService = true
				});

		public static async Task<Models.ServiceInstanceEditable> EnsureDefaultService<I,T>(
			this IServiceInstanceManager Manager,
			long? ParentId,
			object Setting,
			string Name=null,
			string Title=null,
			string Description=null,
			string ServiceIdent=null
			)
		{
			return await Manager.EnsureEntity(
				await TryGetDefaultService<I>(Manager,ParentId),
				() => new Models.ServiceInstanceEditable { },
				e => UpdateServiceModel<I, T>(e, Setting, ParentId,Name, Title, Description, ServiceIdent)
				);
	
		}
		public static async Task<long> TryGetService<I,T>(
			this IServiceInstanceManager Manager,
			long? ParentId = null
			) => await Manager.ResolveEntity(
				new ServiceInstanceQueryArgument
				{
					ServiceType = typeof(I).GetFullName(),
					ImplementId = typeof(T).GetFullName() + "@" + typeof(I).GetFullName(),
					ParentId = ParentId ?? 0,
				});
		
		public static async Task<Models.ServiceInstanceEditable> TryAddService<I, T>(
			this IServiceInstanceManager Manager,
			long? ParentId,
			object Setting,
			string Name = null,
			string Title = null,
			string Description = null,
			string ServiceIdent = null
			)
		{
			var re = Manager.TryGetService<I, T>(ParentId);
			return await Manager.EnsureEntity(
				re?.Id ?? 0,
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
