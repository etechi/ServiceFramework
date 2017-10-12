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
	public class ServiceScopeIdPropertyModifierProvider : IEntityPropertyModifierProvider
	{
		public static int DefaultPriority { get; } = -10000;
		class ServiceScopeIdPropertyModifier : IEntityPropertyModifier<long?>
		{
			public int Priority => DefaultPriority;

			public long? Execute(IDataSetEntityManager Manager, IEntityModifyContext Context)
			{
				return Manager.ServiceInstanceDescroptor?.InstanceId;
			}
		}
		public IEntityPropertyModifier GetPropertyModifier(
			DataActionType ActionType, 
			Type EntityType, 
			PropertyInfo EntityProperty, 
			Type DataModelType, 
			PropertyInfo DataModelProperty
			)
		{   //������long?����
			if (DataModelProperty.PropertyType != typeof(long?))
				return null;

			//û�ж������������ֶ�
			if (DataModelProperty.GetCustomAttribute<ServiceScopeIdAttribute>()==null)
				return null;


			//�Զ����ɷ����ǲ����޸�
			if (ActionType != DataActionType.Create)
				return new NonePropertyModifier(DefaultPriority);

			return new ServiceScopeIdPropertyModifier();
		}
	}
}
