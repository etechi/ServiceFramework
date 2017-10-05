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
			//处理自动生成主键字段

			//主键必须唯一
			var keys = Type.Properties.Where(p => p.CustomAttributes.Any(a => a.Constructor.ReflectedType == typeof(KeyAttribute))).ToArray();
			if (keys.Length != 1)
				return;

			//主键类型必须为long
			if (!(keys[0].PropertyType is SystemTypeReference type))
				return;

			if (type.Type != typeof(long))
				return;

			//必须未定义过数据库生成特性
			if (keys[0].CustomAttributes.Any(a => a.Constructor.ReflectedType == typeof(DatabaseGeneratedAttribute)))
				return;

			//禁止数据库自动主键生成
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
