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
		
		static IEntityMetadata CreateMetadata(string Ident,Type ManagerType)
		{
			var interfaces = ManagerType.AllInterfaces().Where(i=>i.IsGeneric()).ToDictionary(i=>i.GetGenericTypeDefinition());
			var Loadable = interfaces.Get(typeof(IEntityLoadable<,>));
			if (Loadable == null)
				throw new InvalidOperationException($"实体管理类{ManagerType}没有定义IEntityLoadable<,>接口");
			var LoadableArgs = Loadable.GetGenericArguments();

			var BatchLoadable= interfaces.Get(typeof(IEntityBatchLoadable<,>));
			var BatchLoadableArgs = BatchLoadable?.GetGenericArguments();
			if(BatchLoadableArgs!=null && (BatchLoadableArgs[0]!=LoadableArgs[0] || BatchLoadableArgs[1]!=LoadableArgs[1]))
				throw new InvalidOperationException($"实体管理类{ManagerType}的{Loadable}接口和{BatchLoadable}接口类主键或实体类型不一致");

			var Queryable=interfaces.Get(typeof(IEntityQueryable<,,>));
			var QueryableArgs = Queryable?.GetGenericArguments();
			if(QueryableArgs!=null && QueryableArgs[0]!=LoadableArgs[0])
				throw new InvalidOperationException($"实体管理类{ManagerType}的{Loadable}接口和{Queryable}接口类主键类型不一致");

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
				EntityEditable = EditableLoaderArgs[1];
				EntityEditableSource = EditableLoader;
			}
			else if (EntityEditable != EditableLoaderArgs[1])
				throw new InvalidOperationException($"实体管理类{ManagerType}的{EntityEditableSource}接口和{EntityEditable}接口类实体类型不一致");


			var Updatable = interfaces.Get(typeof(IEntityUpdator<,>));
			var UpdatableArgs = Updatable?.GetGenericArguments();
			if (UpdatableArgs != null && UpdatableArgs[0] != LoadableArgs[0])
				throw new InvalidOperationException($"实体管理类{ManagerType}的{Loadable}接口和{Updatable}接口类主键类型不一致");

			if (EntityEditable == null)
			{
				EntityEditable = UpdatableArgs[1];
				EntityEditableSource = Updatable;
			}
			else if (EntityEditable != UpdatableArgs[1])
				throw new InvalidOperationException($"实体管理类{ManagerType}的{EntityEditableSource}接口和{Updatable}接口类实体类型不一致");


			var Removable = interfaces.Get(typeof(IEntityRemover<>));
			var RemovableArgs = Removable?.GetGenericArguments();
			if (RemovableArgs != null && RemovableArgs[0] != LoadableArgs[0])
				throw new InvalidOperationException($"实体管理类{ManagerType}的{Loadable}接口和{RemovableArgs}接口类主键类型不一致");

			var AllRemovable = interfaces.Get(typeof(IEntityAllRemover));

			var comment = ManagerType.Comment();
			return new EntityMetadata
			{
				Ident = Ident??ManagerType.FullName,
				Description = comment.Description,
				Name = comment.Name,
				GroupName = comment.GroupName,
				EntityKeyType = LoadableArgs[0],
				EntityDetailType = LoadableArgs[1],
				EntityEditableType = EntityEditable,
				EntityManagerType = ManagerType,
				EntitySummaryType = QueryableArgs == null ? null : QueryableArgs[1],
				QueryArgumentType = QueryableArgs == null ? null : QueryableArgs[2],
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
			sc.AddSingleton(sp =>
			{
				var svcResolver = sp.Resolve<IServiceDeclarationTypeResolver>();
				return new EntityMetadataCollection(
					from type in sc.GetServiceTypes()
					let em = type.GetCustomAttribute<EntityManagerAttribute>(true)
					where em != null
					select CreateMetadata(em.Ident, type)
					);
			});

			sc.AddScoped<IEntityReferenceResolver, EntityReferenceResolver>();

			sc.Add(typeof(IDataSetEntityManager<>), typeof(DataSetEntityManager<>),ServiceImplementLifetime.Scoped);
			
			return sc;
		}
	}
}
