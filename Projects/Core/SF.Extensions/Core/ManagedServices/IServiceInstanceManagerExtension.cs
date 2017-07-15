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
						DeclarationId = typeof(I).FullName,
						IsDefaultService = true
					});
		}
		public static async Task<Models.ServiceInstanceEditable> EnsureDefaultService<I,T>(
			this IServiceInstanceManager Manager,
			object Setting,
			string Name=null,
			string Title=null,
			string Description=null
			)
		{
			return await Manager.EnsureEntity(
				await Manager.ResolveEntity(
					new ServiceInstanceQueryArgument
					{
						DeclarationId = typeof(I).FullName,
						IsDefaultService = true
					}),
				() => new Models.ServiceInstanceEditable{},
				e=>
				{
					var comment = typeof(T).Comment();
					e.ImplementType = typeof(T).FullName + "@" + typeof(I).FullName;
					e.ServiceType = typeof(I).FullName;
					e.Priority= 0;
					e.ObjectState = Data.LogicObjectState.Enabled;
					//e.SettingType = typeof(T).FullName + "CreateArguments";
					e.Name = Name ?? comment.Name;
					e.Title = Title ?? comment.Name;
					e.DisplayData.Description = Description ?? comment.Description;
					//e.Setting = Json.Stringify(Setting);
				});
	
		}
		public static async Task<Models.ServiceInstanceEditable> TryAddService<I, T>(
			this IServiceInstanceManager Manager,
			object Setting,
			string Name = null,
			string Title = null,
			string Description = null
			)
		{
			var re=await Manager.QueryAsync(
				new ServiceInstanceQueryArgument
				{
					DeclarationId = typeof(I).FullName,
					//ImplementId = typeof(T).FullName + "@" + typeof(I).FullName
				}, Data.Paging.Default
				);
			var items = re.Items.ToArray();
			if (items.Length > 1)
				return null;
			return await Manager.EnsureEntity(
				items.Length==0?0:items[0].Id,
				() => new Models.ServiceInstanceEditable { },
				e =>
				{
					var comment = typeof(T).Comment();
					e.ImplementType = typeof(T).FullName + "@" + typeof(I).FullName;
					e.ServiceType = typeof(I).FullName;
					e.ObjectState = Data.LogicObjectState.Enabled;
					//e.SettingType = typeof(T).FullName + "CreateArguments";
					e.Name = Name ?? comment.Name;
					e.Title = Title ?? comment.Name;
					e.DisplayData = new Data.Models.UIDisplayData
					{
						Description = Description ?? comment.Description
					};
					//e.Setting = Json.Stringify(Setting);
				});

		}
	}
}
