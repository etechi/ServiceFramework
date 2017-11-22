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

			// �������½ṹ, C1�ϵ�C2���Ա�����ڣ�������Ӧ���Բ�ȫ
			// class C1{
			//		[Index]						<--2. ��鲢��ȫ����ֶ�����
			//		public int C2Id{get;set;}	<--1. �������ֶΣ�����ȫ��
			//
			//		[ForeignKey(nameof(Ke1))]	<-- 3. ��鲢��ȫ����ֶ�����
			//		public C2 C2{get;set;}		<--������ڣ��������Բ�ȫ
			// }
			// class C2{
			//		[InvestProperty(nameof(C1.C2))]			<--5. ��鲢��ȫ�����ֶ�����
			//		public ICollection<C1> C1s{get;set;}	<--4. ��鲢��ȫ�����ֶ�
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
				
				//��ȡ�������
				var foreignKeyAttr = singleRelation.prop.Attributes?.SingleOrDefault(a => a.Name == typeof(ForeignKeyAttribute).FullName);
				var foreignKeyName = foreignKeyAttr?.Values?.Get("Name") as string ?? singleRelation.prop.Name + "Id";

				//��������ֶ�
				var keyProperty = typeExpr.Properties.SingleOrDefault(p => p.Name == foreignKeyName);
				if (keyProperty == null)
				{
					errors.Add($"{singleRelation.type.Name}�ϵĶ�������{singleRelation.prop.Name}ȱ������ֶ�{foreignKeyName}");
					continue;
				}

				//��ȡ����ֶ�����
				if (!(keyProperty.PropertyType is SystemTypeReference keyPropType))
				{
					errors.Add($"{singleRelation.type.Name}�ϵĶ�������{singleRelation.prop.Name}����ֶ�{foreignKeyName}������ֵ����");
					continue;
				}

				var targetTypeExpr = Context.TypeExpressions[singleRelation.target.Name];

				//���Ŀ������Ƿ����Ψһ����
				var keys = targetTypeExpr.Properties
					.Where(p => p.CustomAttributes.Any(a => a.Constructor.ReflectedType == typeof(KeyAttribute)))
					.ToArray();
				if(keys.Length>1)
				{
					errors.Add($"{singleRelation.type.Name}�ϵĶ�������{singleRelation.prop.Name}����{singleRelation.target.Name}�г���1�������ֶΣ�{keys.Select(k=>k.Name).Join(",")}");
					continue;
				}

				//���Ŀ�������������
				if(!(keys[0].PropertyType is SystemTypeReference vt))
				{
					errors.Add($"{singleRelation.target.Name}�������ֶΣ�{keys[0].Name}������ֵ����");
					continue;
				}

				//�������������Ƿ�һ��
				var vType = keyPropType.Type;
				if (vType.IsGeneric() && vType.GetGenericTypeDefinition() == typeof(Nullable<>))
					vType = vType.GenericTypeArguments[0];
				if (vType != vt.Type)
				{
					errors.Add($"{singleRelation.type.Name}�ϵĶ�������{singleRelation.prop.Name}����ֶ�{foreignKeyName}����Ϊ{vType}��Ŀ�����{singleRelation.target.Name}�������ֶ�{keys[0].Name}����{vt.Type}��ͬ");
					continue;
				}


				//�����������
				var prop = new PropertyExpression(
					singleRelation.prop.Name,
					new TypeExpressionReference(
						Context.TypeExpressions[singleRelation.target.Name]
						),
					PropertyAttributes.None					
					);

				//��ȫ�������
				prop.CustomAttributes.Add(
					new CustomAttributeExpression(
						typeof(ForeignKeyAttribute).GetConstructor(new[] { typeof(string) }),
						new[] { foreignKeyName }
						)
					);

				//��ȫ����ֶ�����
				DataModelBuildHelper.EnsureSingleFieldIndex(
					typeExpr,
					prop,
					singleRelation.type,
					singleRelation.prop
					);


				//����ʵ���ϵļ��϶���
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

				//������ʵ���϶�����󼯺�
				var collPropName = collProp != null ? collProp.Name : singleRelation.type.Name + "s";
				if(collProp==null && targetTypeExpr.Properties.Any(p=>p.Name==collPropName))
				{
					errors.Add($"�Զ�����{singleRelation.target.Name}�ϵ�{singleRelation.type.Name}���ͼ�������{collPropName}ʱ���ͳ�ͻ���Ѵ���ͬ��������");
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
					errors.Add($"ʵ��{mr.Key.type.Name}�϶����˼�������{p.prop.Name},ָ������{mr.Key.target.Name}������{ira.Values?.Get(nameof(InversePropertyAttribute.Property))}���ԣ���{mr.Key.target.Name}�����ϲ�δ���������");
				}
			}

			if (errors.Count > 0)
				throw new InvalidOperationException("�������ʵ���ϵʽ���������д���:\n" + errors.Join(";\n"));
		}
	}
}
