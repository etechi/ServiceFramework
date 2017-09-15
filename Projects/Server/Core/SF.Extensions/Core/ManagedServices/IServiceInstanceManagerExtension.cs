using System;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using SF.Entities;
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
			string ServiceIdent = null,
			EntityLogicState State=EntityLogicState.Enabled
			)
		{
			var comment = typeof(T).Comment();
			e.ImplementType = typeof(T).GetFullName() + "@" + typeof(I).GetFullName();
			e.ServiceType = typeof(I).GetFullName();
			e.ItemOrder = 0;
			e.LogicState = State;
			e.ServiceIdent = ServiceIdent;
			//e.SettingType = typeof(T).FullName + "CreateArguments";
			e.Name = Name ?? comment.Name;
			e.Title = Title ?? comment.Name;
			e.ContainerId = ParentId;
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
					ContainerId = ParentId ?? 0,
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
			string ServiceIdent=null,
			EntityLogicState State = EntityLogicState.Enabled
			)
		{
			return await Manager.EnsureEntity(
				await TryGetDefaultService<I>(Manager,ParentId),
				() => new Models.ServiceInstanceEditable { },
				e => UpdateServiceModel<I, T>(e, Setting, ParentId,Name, Title, Description, ServiceIdent, State)
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
				e => UpdateServiceModel<I, T>(e, Setting, ParentId, Name, Title, Description, ServiceIdent, State)
				);

		}

		public static async Task SetServiceParent(
			this IServiceInstanceManager Manager,
			long ServiceId,
			long? ParentServiceId
			)
		=> await Manager.UpdateEntity(
				ServiceId,
				e => e.ContainerId=ParentServiceId
				);

	
	
	}
}
