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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data
{
	interface IDataSetProviderResetable
	{
		IDataSetProviderResetable NextResetable { get; set; }
		object SetProvider { get; set; }
	}
	public class DataSet<T> : IDataSet<T>, IDataSetProviderResetable
			where T : class
	{
		public IDataContext Context { get; }
		IDataSetProvider<T> _SetProvider;
		public IDataSetProvider<T> SetProvider
		{
			get
			{
				if(_SetProvider==null)
					_SetProvider=((DataContext)Context).GetSetProvider<T>();
				return _SetProvider;
			}
		}
		IDataSetProviderResetable IDataSetProviderResetable.NextResetable
		{
			get; set;
		}
		object IDataSetProviderResetable.SetProvider
		{
			get
			{
				return _SetProvider;
			}
			set
			{
				_SetProvider = (IDataSetProvider<T>)value;
			}
		}
		public DataSet(IDataContext Context)
		{
			this.Context=Context;
			((DataContext)Context).RegisterDataSet(this);
		}

		public IDataSetMetadata Metadata
		{
			get
			{
				return SetProvider.Metadata;
			}
		}

		public T Add(T Model) => SetProvider.Add(Model);

		public void AddRange(IEnumerable<T> Items) => SetProvider.AddRange(Items);


		public T Remove(T Model) => SetProvider.Remove(Model);

		public void RemoveRange(IEnumerable<T> Items) => SetProvider.RemoveRange(Items);


		public T Update(T item) => SetProvider.Update(item);

		public IContextQueryable<T> AsQueryable(bool ReadOnly) => SetProvider.AsQueryable(ReadOnly);

		public Task<T> FindAsync(params object[] Idents) => SetProvider.FindAsync(Idents);

		public Task<T> FindAsync(object Ident) => SetProvider.FindAsync(Ident);


	}
}
