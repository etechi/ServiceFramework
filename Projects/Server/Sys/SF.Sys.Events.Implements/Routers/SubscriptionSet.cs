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
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections;

namespace SF.Sys.Events
{

	
	class SubscriptionSet : IEnumerable<Subscription>
	{
		Subscription[] _Items;
		int _Count;
		public int Count => _Count;


		public int Add(Subscription subscription)
		{
			var l = _Items?.Length ?? 0;
			if (_Count == l)
				Array.Resize(ref _Items, (l == 0 ? 16 : l) * 2);
			var idx = _Count;
			_Items[_Count] = subscription;
			_Count++;
			return idx;
		}

		public IEnumerator<Subscription> GetEnumerator()
		{
			if (_Items == null)
				yield break;
			var l = _Items.Length;
			//多线程下可能会有重复项目
			var hash = new HashSet<Subscription>();
			for (var i = 0; i < l; i++)
			{
				var s = _Items[i];
				if (s == null)
					break;
				if (hash.Add(s))
					yield return s;
			}
		}

		public void Remove(int Index)
		{
			if (Index < 0 || Index >= _Count)
				throw new ArgumentException();
			var last = _Count - 1;
			var s = _Items[last];
			if (Index < last)
			{
				_Items[Index] = s;
				s.Index = Index;
			}
			_Items[last] = null;
			_Count = last;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
