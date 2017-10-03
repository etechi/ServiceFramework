using System;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using SF.Entities;
using SF.Core.ServiceManagement.Models;

namespace SF.Core.ServiceManagement.Management
{
	public static class IServiceInstanceManagerExtension
	{
		public static async Task<long?> ResolveDefaultService<I>(
			this IServiceInstanceManager Manager
			)
		{
			return (await Manager.QuerySingleEntityIdent(
					new ServiceInstanceQueryArgument
					{
						ServiceId = typeof(I).GetFullName().UTF8Bytes().MD5().Hex(),
						IsDefaultService = true
					}))?.Id;
		}

		static void UpdateServiceModel(Models.ServiceInstanceEditable e,
			Type InterfaceType,
			Type ImplementType,
			object Setting,
			long? ParentId=null,
			string Name = null,
			string Title = null,
			string Description = null,
			string ServiceIdent = null,
			EntityLogicState State=EntityLogicState.Enabled
			)
		{
			e.ImplementId = $"{ImplementType.GetFullName()}@{InterfaceType.GetFullName()}".UTF8Bytes().MD5().Hex();
			e.ImplementType = ImplementType.GetFullName();
			e.ImplementName = ImplementType.FriendlyName();

			e.ServiceId = InterfaceType.GetFullName().UTF8Bytes().MD5().Hex();
			e.ServiceType = InterfaceType.GetFullName();
			e.ServiceName = InterfaceType.FriendlyName();

			e.ItemOrder = 0;
			e.LogicState = State;
			e.ServiceIdent = ServiceIdent;
			//e.SettingType = typeof(T).FullName + "CreateArguments";
			e.Name = Name ?? e.ImplementName;

			var comment = ImplementType.Comment();
			e.Title = Title ?? e.Name;
			e.ContainerId = ParentId;
			e.Description = Description ?? comment?.Description;
			e.Setting = Json.Stringify(Setting);
		}
		public static Task<ObjectKey<long>> TryGetDefaultService<I>(
			this IServiceInstanceManager Manager,
			long? ParentId = null
			) => Manager.TryGetDefaultService(typeof(I), ParentId);

		public static async Task<ObjectKey<long>> TryGetDefaultService(
			this IServiceInstanceManager Manager,
			Type InterfaceType,
			long? ParentId = null
			)
			=> await Manager.QuerySingleEntityIdent(
				new ServiceInstanceQueryArgument
				{
					ContainerId = ParentId ?? 0,
					ServiceId = InterfaceType.GetFullName().UTF8Bytes().MD5().Hex(),
					IsDefaultService = true
				});

		public static Task<Models.ServiceInstanceEditable> EnsureDefaultService<I, T>(
			this IServiceInstanceManager Manager,
			long? ParentId,
			object Setting,
			string Name = null,
			string Title = null,
			string Description = null,
			string ServiceIdent = null,
			EntityLogicState State = EntityLogicState.Enabled
			)
			=> EnsureDefaultService(Manager, ParentId, typeof(I), typeof(T), Setting, Name, Title, Description, ServiceIdent, State);

		public static async Task<Models.ServiceInstanceEditable> EnsureDefaultService(
			this IServiceInstanceManager Manager,
			long? ParentId,
			Type InterfaceType,
			Type ImplementType,
			object Setting,
			string Name = null,
			string Title = null,
			string Description = null,
			string ServiceIdent = null,
			EntityLogicState State = EntityLogicState.Enabled
			)
		{
			return await Manager.EnsureEntity(
				await TryGetDefaultService(Manager, InterfaceType,ParentId),
				() => new Models.ServiceInstanceEditable { },
				e => UpdateServiceModel(e, InterfaceType, ImplementType, Setting, ParentId, Name, Title, Description, ServiceIdent, State)
				);
		}
		public static async Task UpdateDefaultServiceSetting<I>(
			this IServiceInstanceManager Manager,
			long? ParentId,
			Func<string,string> Edit
			)
		{
			var id = await TryGetDefaultService<I>(Manager, ParentId);
			if (id == null)
				throw new ArgumentException($"找不到默认服务{typeof(I)},当前区域:{ParentId}");
			await Manager.UpdateEntity(
				ObjectKey.From(id.Id),
				(ServiceInstanceEditable e) => { e.Setting = Edit(e.Setting); }
				);
		}
		public static async Task<ObjectKey<long>> TryGetService<I,T>(
			this IServiceInstanceManager Manager,
			long? ParentId = null
			) =>await Manager.QuerySingleEntityIdent(
				new ServiceInstanceQueryArgument
				{
					ServiceId = typeof(I).GetFullName().UTF8Bytes().MD5().Hex(),
					ImplementId = (typeof(T).GetFullName() + "@" + typeof(I).GetFullName()).UTF8Bytes().MD5().Hex(),
					ContainerId = ParentId ?? 0,
				});
		
		public static async Task<Models.ServiceInstanceEditable> TryAddService<I, T>(
			this IServiceInstanceManager Manager,
			long? ParentId,
			object Setting,
			string Name = null,
			string Title = null,
			string Description = null,
			string ServiceIdent = null,
			EntityLogicState State = EntityLogicState.Enabled
			)
		{
			var re = await Manager.TryGetService<I, T>(ParentId);
			return await Manager.EnsureEntity(
				re,
				() => new Models.ServiceInstanceEditable { },
				e => UpdateServiceModel(e, typeof(I), typeof(T), Setting, ParentId, Name, Title, Description, ServiceIdent, State)
				);
		}

		public static async Task SetServiceParent(
			this IServiceInstanceManager Manager,
			long ServiceId,
			long? ParentServiceId
			)
		=> await Manager.UpdateEntity(
				Manager,
				ObjectKey.From(ServiceId),
				(ServiceInstanceEditable e) =>e.ContainerId = ParentServiceId
				);

	
	
	}
}
