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
using System.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using SF.Sys.NetworkService;
using System.Net;
using SF.Sys.Services;

namespace SF.Sys.AspNetCore.NetworkServices
{
	class InvokeContext : IInvokeContext, IInvokeRequest, IInvokeResponse
	{
		public Microsoft.AspNetCore.Http.HttpContext Context { get; }

		public InvokeContext(IServiceProvider Services)
		{
			Context = Services.Resolve<IHttpContextAccessor>().HttpContext;
		}

		public IInvokeRequest Request => this;

		public IInvokeResponse Response => this;

		string IInvokeRequest.Method => Context?.Request.Method ;

		string IInvokeRequest.Uri => Context?.Request.GetEncodedUrl();

		HeaderSet RequestHeaders;
		IReadOnlyDictionary<string, IEnumerable<string>> IInvokeRequest.Headers
		{
			get
			{
				if (RequestHeaders == null)
					RequestHeaders = new HeaderSet
					{
						Headers = Context.Request.Headers
					};
				return RequestHeaders;
			}
		}

		string IInvokeResponse.Status
		{
			get
			{
				return Context.Response.StatusCode.ToString();
			}

			set
			{
				Context.Response.StatusCode = int.Parse(value);
			}
		}
		class HeaderSet: IDictionary<string, IEnumerable<string>>, IReadOnlyDictionary<string, IEnumerable<string>>
		{
			public bool IsReadOnly { get; set; }
			public IHeaderDictionary Headers { get; set; }
			public IEnumerable<string> this[string key]
			{
				get { return Headers[key]; }
				set
				{
					Headers.Remove(key);
					Headers.Add(key, new StringValues(value.ToArray()));
				}
			}

			public int Count => Headers.Count();

			public ICollection<string> Keys => Headers.Select(p => p.Key).ToArray();

			public ICollection<IEnumerable<string>> Values=> Headers.Select(p => p.Value).Cast<IEnumerable<string>>().ToArray();

			IEnumerable<string> IReadOnlyDictionary<string, IEnumerable<string>>.Keys => Headers.Select(p => p.Key);

			IEnumerable<IEnumerable<string>> IReadOnlyDictionary<string, IEnumerable<string>>.Values => Headers.Select(p => p.Value).Cast<IEnumerable<string>>();

			public void Add(KeyValuePair<string, IEnumerable<string>> item)
			{
				Headers.Add(item.Key, new StringValues(item.Value.ToArray()));
			}

			public void Add(string key, IEnumerable<string> value)
			{
				Headers.Add(key, new StringValues(value.ToArray()));
			}

			public void Clear()
			{
				Headers.Clear();
			}

			public bool Contains(KeyValuePair<string, IEnumerable<string>> item)
			{
				StringValues v;
				return Headers.TryGetValue(item.Key, out v) && v==new StringValues(item.Value.ToArray());
			}

			public bool ContainsKey(string key)
			{
				StringValues v;
				return Headers.TryGetValue(key,out v);
			}

			public void CopyTo(KeyValuePair<string, IEnumerable<string>>[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
			{
				return Headers.Select(p=>new KeyValuePair<string, IEnumerable<string>>(p.Key,p.Value)).GetEnumerator();
			}

			public bool Remove(KeyValuePair<string, IEnumerable<string>> item)
			{
				StringValues v;
				if (Headers.TryGetValue(item.Key, out v) && v == new StringValues(item.Value.ToArray()))
				{
					Headers.Remove(item.Key);
					return true;
				}
				return false;
			}

			public bool Remove(string key)
			{
				return Headers.Remove(key);
			}

			public bool TryGetValue(string key, out IEnumerable<string> value)
			{
				StringValues v;
				if (Headers.TryGetValue(key, out v))
				{
					value = v;
					return true;
				}
				else
				{
					value = null;
					return false;
				}
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
						Headers = Context.Response.Headers
					};
				return ResponseHeaders;
			}
		}

		string IInvokeRequest.GetCookie(string key)
		{
			return Context.Request.Cookies.TryGetValue(key, out var re) ? re : null;
		}

		void IInvokeResponse.SetCookie(Cookie Cookie)
		{
			if (Cookie == null)
				throw new ArgumentNullException();
			var options = new CookieOptions
			{
				Domain = Cookie.Domain,
				Expires = Cookie.Expires.Year<2000?new DateTime(2000,1,1):Cookie.Expires,
				HttpOnly = Cookie.HttpOnly,
				Path = Cookie.Path,
				SameSite = SameSiteMode.Lax,
				Secure = Cookie.Secure
			};
			if(Cookie.Value==null)
				Context.Response.Cookies.Delete(Cookie.Name, options);
			else
				Context.Response.Cookies.Append(Cookie.Name, Cookie.Value, options);
		}
		
	}



}
