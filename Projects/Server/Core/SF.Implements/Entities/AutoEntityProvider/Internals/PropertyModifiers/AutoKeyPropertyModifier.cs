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
		{   //������long����
			if (DataModelProperty.PropertyType != typeof(long))
				return null;

			//���������ֶ�
			if (!DataModelProperty.IsDefined(typeof(KeyAttribute)))
				return null;

		
			//ʹ�����ݿ�����
			if ((DataModelProperty.GetCustomAttribute<DatabaseGeneratedAttribute>()?.DatabaseGeneratedOption ?? DatabaseGeneratedOption.Identity) != DatabaseGeneratedOption.None)
				return null;

			//ʹ�����ݿ�����
			if (EntityProperty.GetCustomAttribute<DatabaseGeneratedAttribute>() != null)
				return null;

			//�ж�������ֶ�
			if (DataModelType.AllPublicInstanceProperties().Any(p => p != DataModelProperty && p.IsDefined(typeof(KeyAttribute))))
				return null;

			//�Զ��������������޸�
			if (ActionType != DataActionType.Create)
				return new NonePropertyModifier(DefaultPriority);

			return new AutoKeyPropertyModifier();
		}
	}
}
