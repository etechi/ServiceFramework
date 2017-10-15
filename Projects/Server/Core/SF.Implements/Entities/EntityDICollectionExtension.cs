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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SF.Metadata;
using SF.Entities;

namespace SF.Core.ServiceManagement
{
	public static class EntityDICollectionExtension
	{
		
		static IEntityMetadata CreateMetadata(string GroupIdent,string GroupName,IEntityServiceDescriptor desc)
		{
			Type ManagerType = desc.ServiceType;
			var Ident = desc.Ident ?? GroupIdent;// ManagerType.GetFirstInterfaceAttribute<EntityManagerAttribute>()?.Ident;

			var interfaces = ManagerType.AllInterfaces().Where(i=>i.IsGeneric()).ToDictionary(i=>i.GetGenericTypeDefinition());
			var Loadable = interfaces.Get(typeof(IEntityLoadable<,>));
			if (Loadable == null)
				throw new InvalidOperationException($"实体管理类{ManagerType}没有定义IEntityLoadable<,>接口");
			var LoadableArgs = Loadable.GetGenericArguments();

			var BatchLoadable= interfaces.Get(typeof(IEntityBatchLoadable<,>));
			var BatchLoadableArgs = BatchLoadable?.GetGenericArguments();
			if(BatchLoadableArgs!=null && (BatchLoadableArgs[0]!=LoadableArgs[0] || BatchLoadableArgs[1]!=LoadableArgs[1]))
				throw new InvalidOperationException($"实体管理类{ManagerType}的{Loadable}接口和{BatchLoadable}接口类主键或实体类型不一致");

			var Queryable=interfaces.Get(typeof(IEntityQueryable<,>));
			var QueryableArgs = Queryable?.GetGenericArguments();
			//if(QueryableArgs!=null && QueryableArgs[0]!=LoadableArgs[0])
			//	throw new InvalidOperationException($"实体管理类{ManagerType}的{Loadable}接口和{Queryable}接口类主键类型不一致");

			var Creatable = interfaces.Get(typeof(IEntityCreator<,>));
			var CreatableArgs = Creatable?.GetGenericArguments();
			if (CreatableArgs != null && CreatableArgs[0] != LoadableArgs[0])
				throw new InvalidOperationException($"实体管理类{ManagerType}的{Loadable}接口和{Creatable}接口类主键类型不一致");

			var EntityEditable = CreatableArgs == null ? null : CreatableArgs[1];
			var EntityEditableSource= Creatable == null ? null : Creatable;

			var EditableLoader = interfaces.Get(typeof(IEntityEditableLoader<,>));
			var EditableLoaderArgs = EditableLoader?.GetGenericArguments();
			if (EditableLoaderArgs != null && EditableLoaderArgs[0] != LoadableArgs[0])
				throw new InvalidOperationException($"实体管理类{ManagerType}的{Loadable}接口和{EditableLoader}接口类主键类型不一致");

			if (EntityEditable == null)
			{
				if (EditableLoader != null)
				{
					EntityEditable = EditableLoaderArgs[1];
					EntityEditableSource = EditableLoader;
				}
			}
			else if (EntityEditable != EditableLoaderArgs[1])
				throw new InvalidOperationException($"实体管理类{ManagerType}的{EntityEditableSource}接口和{EntityEditable}接口类实体类型不一致");


			var Updatable = interfaces.Get(typeof(IEntityUpdator<>));
			var UpdatableArgs = Updatable?.GetGenericArguments();
			//if (UpdatableArgs != null && UpdatableArgs[0] != LoadableArgs[0])
			//	throw new InvalidOperationException($"实体管理类{ManagerType}的{Loadable}接口和{Updatable}接口类主键类型不一致");

			if (EntityEditable == null)
			{
				if (Updatable != null)
				{
					EntityEditable = UpdatableArgs[0];
					EntityEditableSource = Updatable;
				}
			}
			else if (EntityEditable != UpdatableArgs[0])
				throw new InvalidOperationException($"实体管理类{ManagerType}的{EntityEditableSource}接口和{Updatable}接口类实体类型不一致");


			var Removable = interfaces.Get(typeof(IEntityRemover<>));
			var RemovableArgs = Removable?.GetGenericArguments();
			if (RemovableArgs != null && RemovableArgs[0] != LoadableArgs[0])
				throw new InvalidOperationException($"实体管理类{ManagerType}的{Loadable}接口和{RemovableArgs}接口类主键类型不一致");

			var AllRemovable = interfaces.Get(typeof(IEntityAllRemover));

			var comment = ManagerType.Comment();


			return new EntityMetadata
			{
				Ident = Ident,
				GroupIdent = GroupIdent,
				Description = comment.Description,
				Name = comment?.Name ?? desc.Name ?? comment.GroupName ?? GroupName,
				GroupName = comment.GroupName ?? GroupName,
				EntityKeyType = LoadableArgs[0],
				EntityDetailType = LoadableArgs[1],
				EntityEditableType = EntityEditable,
				EntityManagerType = ManagerType,
				EntitySummaryType = QueryableArgs == null ? null : QueryableArgs[0],
				QueryArgumentType = QueryableArgs == null ? null : QueryableArgs[1],

				EntityTypes = new[] {
					LoadableArgs[1],
					EntityEditable,
					QueryableArgs == null ? null : QueryableArgs[0]
				}.Concat(desc.EntityTypes)
				.Where(t=>t!=null).Distinct().ToArray(),

				EntityManagerCapability =
					EntityCapability.Loadable |
					(BatchLoadable == null ? EntityCapability.None : EntityCapability.BatchLoadable) |
					(Queryable == null ? EntityCapability.None : EntityCapability.Queryable) |
					(Creatable == null ? EntityCapability.None : EntityCapability.Creatable) |
					(Updatable == null ? EntityCapability.None : EntityCapability.Updatable) |
					(EditableLoader == null ? EntityCapability.None : EntityCapability.EditableLoadable) |
					(Removable == null ? EntityCapability.None : EntityCapability.Removable) |
					(AllRemovable == null ? EntityCapability.None : EntityCapability.AllRemovable)
			};
		}
		public static IServiceCollection AddDataEntityProviders(this IServiceCollection sc)
		{
			sc.AddSingleton<IMetadataAttributeValuesProvider<EntityIdentAttribute>, EntityIdentAttributeMetadataValuesProvider>();
			sc.AddSingleton<IMetadataAttributeValuesProvider<EntityManagerAttribute>, EntityManagerAttributeMetadataValuesProvider>();
			sc.AddSingleton<IMetadataAttributeValuesProvider<EntityObjectAttribute>, EntityObjectAttributeMetadataValuesProvider>();
			sc.AddSingleton<IEntityMetadataCollection>(sp =>
			{
				var ems = (from grp in sp.Resolve<IEnumerable<IEntityServiceDescriptorGroup>>()
						   from desc in grp
						   select (grp, desc)
						 ).ToArray();
				return new EntityMetadataCollection(
					ems.Select(em=>CreateMetadata(em.grp.Ident, em.grp.Name, em.desc))
					);
			});

			sc.AddScoped<IEntityReferenceResolver, EntityReferenceResolver>();

			sc.Add(typeof(IDataSetEntityManager<,>), typeof(DataSetEntityManager<,>),ServiceImplementLifetime.Transient);
			
			return sc;
		}
	}
}
