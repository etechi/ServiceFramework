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
	public class BaseSkipPropertyModifierProvider
	{
		protected abstract class BaseSkipPropertyModifier<T,E> : IAsyncEntityPropertyModifier<T,E>
		{
			public int Priority => -1000;
			Func<IDataSetEntityManager, IEntityModifyContext, E, T,Task<E>> GetValue;
			protected abstract bool ShouldSkipNextModifier(T Value);
			public Task<E> Execute(IDataSetEntityManager Manager, IEntityModifyContext Context, E OrgValue, T Value)
			{
				if (GetValue == null)
					throw new Exception("未找到实际的属性修改器");
				if (ShouldSkipNextModifier(Value))
					return Task.FromResult(OrgValue);
				return GetValue(Manager, Context, OrgValue, Value);
			}
			public IEntityPropertyModifier Merge(IEntityPropertyModifier LowPriorityModifier)
			{
				if (LowPriorityModifier is IAsyncEntityPropertyModifier<T, E> m1)
					GetValue = m1.Execute;
				else if (LowPriorityModifier is IAsyncEntityPropertyModifier<E> m2)
					GetValue = (m, c, o, v) => m2.Execute(m, c, o);
				else if (LowPriorityModifier is IEntityPropertyModifier<T, E> m3)
					GetValue = (m, c, o, v) => Task.FromResult(m3.Execute(m, c, o, v));
				else if (LowPriorityModifier is IEntityPropertyModifier<E> m4)
					GetValue = (m, c, o, v) => Task.FromResult(m4.Execute(m, c, o));
				else
					throw new NotSupportedException();
				return this;
			}
		}
		
	}
}
