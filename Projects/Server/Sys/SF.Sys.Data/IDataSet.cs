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
using System.Threading.Tasks;

namespace SF.Sys.Data
{
	public interface IDataSet
	{
		IDataContext Context { get; }
		IDataSetMetadata Metadata { get; }
	}

	public interface IDataSet<T> : IDataSet
		where T : class
	{
		Task<T> FindAsync(object Ident);
		Task<T> FindAsync(params object[] Idents);

		T Add(T Model);
		void AddRange(IEnumerable<T> Items);

		T Remove(T Model);
		void RemoveRange(IEnumerable<T> Items);

		T Update(T Model);

		IContextQueryable<T> AsQueryable(bool ReadOnly=true);
	}
}
