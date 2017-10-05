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
using System.Linq.TypeExpressions;

namespace SF.Entities.AutoEntityProvider.Internals.DataModelBuilders.Providers
{


	public class DataModelAutoPrimaryKeyProvider : IDataModelTypeBuildProvider
	{
		public int Priority => 100;

		public void AfterBuildType(IDataModelBuildContext Context, TypeExpression Type, IEntityType Entity)
		{
			//�����Զ����������ֶ�

			//��������Ψһ
			var keys = Type.Properties.Where(p => p.CustomAttributes.Any(a => a.Constructor.ReflectedType == typeof(KeyAttribute))).ToArray();
			if (keys.Length != 1)
				return;

			//�������ͱ���Ϊlong
			if (!(keys[0].PropertyType is SystemTypeReference type))
				return;

			if (type.Type != typeof(long))
				return;

			//����δ��������ݿ���������
			if (keys[0].CustomAttributes.Any(a => a.Constructor.ReflectedType == typeof(DatabaseGeneratedAttribute)))
				return;

			//��ֹ���ݿ��Զ���������
			keys[0].CustomAttributes.Add(
				new CustomAttributeExpression(
				typeof(DatabaseGeneratedAttribute).GetConstructor(new[] { typeof(DatabaseGeneratedOption) }),
				new object[] { DatabaseGeneratedOption.None }
				)
				);
		}

		public void BeforeBuildType(IDataModelBuildContext Context, TypeExpression Type, IEntityType Entity)
		{
			
		}
	}
}
