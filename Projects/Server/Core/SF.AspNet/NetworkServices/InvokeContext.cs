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
using System.Net.Http;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using SF.Core.Serialization;
using SF.Auth;
using SF.Metadata;
using System.IO;
using SF.Core.NetworkService;
using System.Text;
using System.Web.Http.Results;
using System.Net;
using System.Collections;

namespace SF.AspNet.NetworkService
{
	class InvokeContext : IInvokeContext, IInvokeRequest, IInvokeResponse
	{
		public ServiceController Controller { get; }

		public IInvokeRequest Request => this;

		public IInvokeResponse Response => this;

		string IInvokeRequest.Method => Controller.Request.Method.Method;

		string IInvokeRequest.Uri => Controller.Request.RequestUri.ToString();

		HeaderSet RequestHeaders;
		IReadOnlyDictionary<string, IEnumerable<string>> IInvokeRequest.Headers
		{
			get
			{
				if (RequestHeaders == null)
					RequestHeaders = new HeaderSet
					{
						Headers = Controller.Request.Headers
					};
				return RequestHeaders;
			}
		}

		string IInvokeResponse.Status
		{
			get
			{
				return Controller.ActionContext.Response.StatusCode.ToString();
			}

			set
			{
				Controller.ActionContext.Response.StatusCode = (HttpStatusCode)int.Parse(value);
			}
		}
		class HeaderSet: IDictionary<string, IEnumerable<string>>, IReadOnlyDictionary<string, IEnumerable<string>>
		{
			public bool IsReadOnly { get; set; }
			public System.Net.Http.Headers.HttpHeaders Headers { get; set; }
			public IEnumerable<string> this[string key]
			{
				get { return Headers.GetValues(key); }
				set
				{
					Headers.Remove(key);
					Headers.Add(key, value);
				}
			}

			public int Count => Headers.Count();

			public ICollection<string> Keys => Headers.Select(p => p.Key).ToArray();

			public ICollection<IEnumerable<string>> Values=> Headers.Select(p => p.Value).ToArray();

			IEnumerable<string> IReadOnlyDictionary<string, IEnumerable<string>>.Keys => Headers.Select(p => p.Key);

			IEnumerable<IEnumerable<string>> IReadOnlyDictionary<string, IEnumerable<string>>.Values => Headers.Select(p => p.Value);

			public void Add(KeyValuePair<string, IEnumerable<string>> item)
			{
				Headers.Add(item.Key, item.Value);
			}

			public void Add(string key, IEnumerable<string> value)
			{
				Headers.Add(key, value);
			}

			public void Clear()
			{
				Headers.Clear();
			}

			public bool Contains(KeyValuePair<string, IEnumerable<string>> item)
			{
				return Headers.Contains(item);
			}

			public bool ContainsKey(string key)
			{
				return Headers.Contains(key);
			}

			public void CopyTo(KeyValuePair<string, IEnumerable<string>>[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
			{
				return Headers.GetEnumerator();
			}

			public bool Remove(KeyValuePair<string, IEnumerable<string>> item)
			{
				throw new NotSupportedException();
			}

			public bool Remove(string key)
			{
				return Headers.Remove(key);
			}

			public bool TryGetValue(string key, out IEnumerable<string> value)
			{
				return Headers.TryGetValues(key, out value);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
		HeaderSet ResponseHeaders;
		IDictionary<string, IEnumerable<string>>  IInvokeResponse.Headers
		{
			get
			{
				if (ResponseHeaders == null)
					ResponseHeaders = new HeaderSet
					{
						Headers = Controller.ActionContext.Response.Headers
					};
				return ResponseHeaders;
			}
		}

		public InvokeContext(ControllerSource ControllerSource)
		{
			this.Controller = ControllerSource.Controller;
		}
		
	}



}
