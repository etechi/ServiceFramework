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

namespace SF.Entities.AutoEntityProvider.Internals.DataModelBuilders
{
	public static class DataModelBuildHelper
	{
		public static CustomAttributeExpression[] ToExpressions(
			this NamedServiceResolver<IDataModelAttributeGenerator> Resolver,
			IEnumerable<IAttribute> Attributes
			)
		{
			return Attributes.Select(a => Resolver(a.Name)?.Generate(a)).Where(a => a != null).ToArray();
		}


		public static bool EnsureSingleFieldIndex(
			TypeExpression typeExpr,
			PropertyExpression propExpr, 
			IEntityType type, 
			IProperty prop
			)
		{
			//�Ѿ�������
			int idx;
			if (propExpr.CustomAttributes.Any(a =>
					a.Constructor.ReflectedType == typeof(IndexAttribute) &&  //����
					(
					(a.Arguments.Count() == 0 && a.InitProperties.Count() == 0)//��һ����
					|| (-1 != (idx = a.InitProperties.IndexOf(p => p.Name == "Order")) && (1 == Convert.ToInt32(a.InitPropertyValues.At(idx)))) //����������һ���һ�����
					|| (a.Arguments.Count() == 2 && (1 == Convert.ToInt32(a.Arguments.At(1)))) //����������һ��ڶ������
					)
				))
				return false;

			//��ǰ�ֶ���Ψһ����,�����ǵ�һ��һ������
			var keys = typeExpr.Properties
				.Where(p => p.CustomAttributes.Any(aa => aa.Constructor.ReflectedType == typeof(KeyAttribute)))
				.ToArray();

			//����ֻ��һ���ֶ�
			if (keys.Length == 1 && keys[0] == propExpr)
				return false;

			//�����ж���ֶΣ���ǰ�ֶ��ǵ�һ���ֶ�
			var column = propExpr.CustomAttributes.FirstOrDefault(a => a.Constructor.ReflectedType == typeof(ColumnAttribute));
			if (column != null)
			{
				var propIndex = column.InitProperties.IndexOf(p => p.Name == nameof(ColumnAttribute.Order));
				if (propIndex != -1 && Convert.ToInt32(column.InitPropertyValues.At(propIndex)) == 1)
					return false;
			}
			
			//��������
			propExpr.CustomAttributes.Add(
				new CustomAttributeExpression(typeof(IndexAttribute).GetConstructor(Array.Empty<Type>()))
				);
			return true;
		}
	}
}
