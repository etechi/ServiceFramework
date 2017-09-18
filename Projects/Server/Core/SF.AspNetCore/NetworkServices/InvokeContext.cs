﻿using System;
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
using System.Net;
using System.Collections;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;

namespace SF.AspNetCore.NetworkServices
{
	class InvokeContext : IInvokeContext, IInvokeRequest, IInvokeResponse
	{
		public Microsoft.AspNetCore.Http.HttpContext Context { get; }
		public InvokeContext(Microsoft.AspNetCore.Http.IHttpContextAccessor HttpContextAccessor)
		{
			Context = HttpContextAccessor.HttpContext;
		}

		public IInvokeRequest Request => this;

		public IInvokeResponse Response => this;

		string IInvokeRequest.Method => Context.Request.Method;

		string IInvokeRequest.Uri => Context.Request.GetEncodedUrl();

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

	}



}