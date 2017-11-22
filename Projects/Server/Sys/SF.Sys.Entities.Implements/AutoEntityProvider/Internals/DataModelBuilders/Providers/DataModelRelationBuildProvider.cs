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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Reflection;
using SF.Sys.Collections.Generic;
using SF.Sys.Linq;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.DataModelBuilders.Providers
{


	public class DataModelRelationBuildProvider : IDataModelBuildProvider
	{
		public int Priority => 10;

		public void AfterBuildModel(IDataModelBuildContext Context)
		{
			
		}

		public void BeforeBuildModel(IDataModelBuildContext Context)
		{
			var entityTypes = Context.Metadata.EntityTypes;
			var relations =
				(from et in Context.EntityTypes
				 from p in et.Properties
				 where p.Mode == PropertyMode.SingleRelation || p.Mode == PropertyMode.MultipleRelation
				 select (type: et, prop: p, target: entityTypes[((IEntityType)p.Type).Name])
				).ToArray();

			// 对于如下结构, C1上的C2属性必须存在，其他相应可以补全
			// class C1{
			//		[Index]						<--2. 检查并补全外键字段索引
			//		public int C2Id{get;set;}	<--1. 检查外键字段（不补全）
			//
			//		[ForeignKey(nameof(Ke1))]	<-- 3. 检查并补全外键字段特性
			//		public C2 C2{get;set;}		<--必须存在，其他可以补全
			// }
			// class C2{
			//		[InvestProperty(nameof(C1.C2))]			<--5. 检查并补全集合字段特性
			//		public ICollection<C1> C1s{get;set;}	<--4. 检查并补全集合字段
			// }
			//
			var multiRelations = relations
				.Where(r => r.prop.Mode == PropertyMode.MultipleRelation)
				.GroupBy(r => (r.type, r.target))
				.ToDictionary(g => g.Key);

			var usedMultiRelations = new HashSet<IProperty>();

			var errors = new List<string>();
			foreach(var singleRelation in relations.Where(r=>r.prop.Mode==PropertyMode.SingleRelation))
			{
				var typeExpr = Context.TypeExpressions[singleRelation.type.Name];
				
				//获取外键名称
				var foreignKeyAttr = singleRelation.prop.Attributes?.SingleOrDefault(a => a.Name == typeof(ForeignKeyAttribute).FullName);
				var foreignKeyName = foreignKeyAttr?.Values?.Get("Name") as string ?? singleRelation.prop.Name + "Id";

				//查找外键字段
				var keyProperty = typeExpr.Properties.SingleOrDefault(p => p.Name == foreignKeyName);
				if (keyProperty == null)
				{
					errors.Add($"{singleRelation.type.Name}上的对象属性{singleRelation.prop.Name}缺少外键字段{foreignKeyName}");
					continue;
				}

				//获取外键字段类型
				if (!(keyProperty.PropertyType is SystemTypeReference keyPropType))
				{
					errors.Add($"{singleRelation.type.Name}上的对象属性{singleRelation.prop.Name}外键字段{foreignKeyName}不是数值类型");
					continue;
				}

				var targetTypeExpr = Context.TypeExpressions[singleRelation.target.Name];

				//检查目标对象是否仅有唯一主键
				var keys = targetTypeExpr.Properties
					.Where(p => p.CustomAttributes.Any(a => a.Constructor.ReflectedType == typeof(KeyAttribute)))
					.ToArray();
				if(keys.Length>1)
				{
					errors.Add($"{singleRelation.type.Name}上的对象属性{singleRelation.prop.Name}类型{singleRelation.target.Name}有超过1个主键字段：{keys.Select(k=>k.Name).Join(",")}");
					continue;
				}

				//检查目标对象主键类型
				if(!(keys[0].PropertyType is SystemTypeReference vt))
				{
					errors.Add($"{singleRelation.target.Name}的主键字段：{keys[0].Name}不是数值类型");
					continue;
				}

				//检查外键和主键是否一致
				var vType = keyPropType.Type;
				if (vType.IsGeneric() && vType.GetGenericTypeDefinition() == typeof(Nullable<>))
					vType = vType.GenericTypeArguments[0];
				if (vType != vt.Type)
				{
					errors.Add($"{singleRelation.type.Name}上的对象属性{singleRelation.prop.Name}外键字段{foreignKeyName}类型为{vType}和目标对象{singleRelation.target.Name}的主键字段{keys[0].Name}类型{vt.Type}不同");
					continue;
				}


				//定义对象属性
				var prop = new PropertyExpression(
					singleRelation.prop.Name,
					new TypeExpressionReference(
						Context.TypeExpressions[singleRelation.target.Name]
						),
					PropertyAttributes.None					
					);

				//补全外键特性
				prop.CustomAttributes.Add(
					new CustomAttributeExpression(
						typeof(ForeignKeyAttribute).GetConstructor(new[] { typeof(string) }),
						new[] { foreignKeyName }
						)
					);

				//补全外键字段索引
				DataModelBuildHelper.EnsureSingleFieldIndex(
					typeExpr,
					prop,
					singleRelation.type,
					singleRelation.prop
					);


				//查找实体上的集合对象
				var mr = multiRelations.Get((singleRelation.target, singleRelation.type));
				var collProp = mr == null ? (IProperty)null :
					mr.Count() == 1 ? mr.First().prop :
					mr.Single(r =>
						r.prop.Attributes?.Any(a =>
							a.Name == typeof(InversePropertyAttribute).FullName &&
							Convert.ToString(a.Values?.Get(nameof(InversePropertyAttribute.Property))) == singleRelation.prop.Name
							) ?? false
						).prop;

				if (collProp != null)
					usedMultiRelations.Add(collProp);

				//在数据实体上定义对象集合
				var collPropName = collProp != null ? collProp.Name : singleRelation.type.Name + "s";
				if(collProp==null && targetTypeExpr.Properties.Any(p=>p.Name==collPropName))
				{
					errors.Add($"自动生成{singleRelation.target.Name}上的{singleRelation.type.Name}类型集合属性{collPropName}时发送冲突，已存在同名的属性");
					continue;
				}

				var collPropExpr = new PropertyExpression(
					collPropName,
					new GenericTypeReference(
						new SystemTypeReference(typeof(ICollection<>)),
						new TypeExpressionReference(typeExpr)
						),
					PropertyAttributes.None
					);

				collPropExpr.CustomAttributes.Add(
					new CustomAttributeExpression(
						typeof(InversePropertyAttribute).GetConstructor(new[] { typeof(string) }),
						new[] { singleRelation.prop.Name }
						)
					);

				targetTypeExpr.Properties.Add(collPropExpr);
			}

			foreach(var mr in multiRelations)
			{
				foreach(var p in mr.Value)
				{
					if (usedMultiRelations.Contains(p.prop))
						continue;
					var ira = p.prop.Attributes?.FirstOrDefault(a => a.Name == typeof(InversePropertyAttribute).FullName);
					if (ira == null)
						continue;
					errors.Add($"实体{mr.Key.type.Name}上定义了集合属性{p.prop.Name},指定关联{mr.Key.target.Name}类型上{ira.Values?.Get(nameof(InversePropertyAttribute.Property))}属性，但{mr.Key.target.Name}类型上并未定义该属性");
				}
			}

			if (errors.Count > 0)
				throw new InvalidOperationException("处理对象实体关系式，发生下列错误:\n" + errors.Join(";\n"));
		}
	}
}
