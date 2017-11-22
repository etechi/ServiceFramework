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

using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Reflection;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.DataModelBuilders.Providers
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
