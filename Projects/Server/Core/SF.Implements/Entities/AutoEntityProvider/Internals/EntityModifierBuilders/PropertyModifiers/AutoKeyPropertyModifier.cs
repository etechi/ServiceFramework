using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using SF.Metadata;
using System.Reflection.Emit;
using SF.Core.ServiceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Entities.AutoEntityProvider.Internals.EntityModifiers
{
	public class AutoKeyPropertyModifierProvider : IEntityPropertyModifierProvider
	{
		public static int DefaultPriority { get; } = -10000;
		class AutoKeyPropertyModifier : IAsyncEntityPropertyModifier<long>
		{
			public int Priority => DefaultPriority;

			public Task<long> Execute(IDataSetEntityManager Manager, IEntityModifyContext Context)
			{
				return Manager.IdentGenerator.GenerateAsync(
					Manager.ServiceInstanceDescroptor.ServiceDeclaration.ServiceType.FullName
					);
			}
		}
		public IEntityPropertyModifier GetPropertyModifier(
			DataActionType ActionType, 
			Type EntityType, 
			PropertyInfo EntityProperty, 
			Type DataModelType, 
			PropertyInfo DataModelProperty
			)
		{   //必须是long类型
			if (DataModelProperty.PropertyType != typeof(long))
				return null;

			//不是主键字段
			if (!DataModelProperty.IsDefined(typeof(KeyAttribute)))
				return null;

		
			//使用数据库生成
			if ((DataModelProperty.GetCustomAttribute<DatabaseGeneratedAttribute>()?.DatabaseGeneratedOption ?? DatabaseGeneratedOption.Identity) != DatabaseGeneratedOption.None)
				return null;

			//使用数据库生成
			if (EntityProperty.GetCustomAttribute<DatabaseGeneratedAttribute>() != null)
				return null;

			//有多个主键字段
			if (DataModelType.AllPublicInstanceProperties().Any(p => p != DataModelProperty && p.IsDefined(typeof(KeyAttribute))))
				return null;

			//自动生成主键不能修改
			if (ActionType != DataActionType.Create)
				return new NonePropertyModifier(DefaultPriority);

			return new AutoKeyPropertyModifier();
		}
	}
}
