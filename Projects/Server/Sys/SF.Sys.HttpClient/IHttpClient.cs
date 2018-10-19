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
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.HttpClients
{
	public interface IHttpClient
	{
		Task<T> Request<T>(HttpRequestMessage Request, Func<HttpResponseMessage, Task<T>> GetResult);
	}
    
	public static class HttpClientExtension
	{
		public class Request
		{
			public HttpMethod Method { get; set; }
			public IHttpClient HttpClient { get; set; }
			public Uri Uri { get; set; }
			public HttpContent Content { get; set; }
			public bool EnsureSuccess { get; set; }
			public List<Action<HttpResponseMessage>> OnReturns { get; set; }
			public IEnumerable<(string Name, string Value)> Headers { get; set; }
		}
		public static HttpRequestMessage GetRequestMessage(this Request request)
		{
			var req = new HttpRequestMessage(
				request.Method ??
					(request.Content == null ? HttpMethod.Get : HttpMethod.Post),
				request.Uri
				);
			if (request.Content != null)
				req.Content = request.Content;
			if (request.Headers != null)
				foreach (var h in request.Headers)
					req.Headers.Add(h.Name, h.Value);
			return req;
		}
		public static Request From(this IHttpClient Client, string Uri)
			=> Client.From(new Uri(Uri));
		public static Request From(this IHttpClient Client, Uri Uri)
		{
			return new Request
			{
				Uri = Uri,
				HttpClient = Client,
				EnsureSuccess = true
			};
		}
		public static Request WithMethod(this Request Request, HttpMethod Method)
		{
			Request.Method = Method;
			return Request;
		}
		public static Request WithContent(this Request Request, string Content, string mime = null, Encoding Encoding = null)
		{
			Request.Content = new StringContent(Content, Encoding ?? Encoding.UTF8, mime ?? "application/json");
			return Request;
		}
        public static HttpContent WithHeaders(this HttpContent content, string charset = null, string file = null, string name = null, string mime = null,string disposition=null)
        {
            if (mime != null || charset != null)
            {
                if (content.Headers.ContentType == null)
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mime ?? "text/plain");
                else if (mime != null)
                    content.Headers.ContentType.MediaType = mime;
            
                if (charset != null)
                    content.Headers.ContentType.CharSet = charset;
            }
            if (file != null || name != null)
            {
                if (content.Headers.ContentDisposition == null)
                    content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue(disposition??"attachment");
                if (file != null)
                    content.Headers.ContentDisposition.FileName = file;
                if (name != null)
                    content.Headers.ContentDisposition.Name = name;

            }
            return content;
        }

        public static ByteArrayContent CreateContent(byte[] content, string mime,string charset=null, string file=null, string name=null)
        {
            var ctn = new ByteArrayContent(content);
            ctn.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mime);
            if (charset != null)
                ctn.Headers.ContentType.CharSet = charset;
            if (file!=null || name!=null)
                ctn.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = file,
                    Size = content.Length,
                    Name = name
                };
            return ctn;
        }
        public static Request WithMultipartFormDataContent(this Request Request, HttpContent[] Contents)
        {
            var mctn = new MultipartFormDataContent();
            foreach(var ctn in Contents)
                mctn.Add(ctn);
            Request.Content = mctn;
            
            return Request;
        }
		public static Request OnReturn(this Request Request, Action<HttpResponseMessage> Callback)
		{
			if (Request.OnReturns == null)
				Request.OnReturns = new List<Action<HttpResponseMessage>>();
			Request.OnReturns.Add(Callback);
			return Request;
		}
		public static Request WithContent(this Request Request, HttpContent Content)
		{
			Request.Content = Content;
			return Request;
		}
		public static Request WithHeader(this Request Request, string Name, string Value)
			=> Request.WithHeaders((Name, Value));

		public static Request WithHeaders(this Request Request, params (string Name, string Value)[] Headers)
			=> Request.WithHeaders((IEnumerable<(string Name, string Value)>)Headers);

		public static Request WithHeaders(this Request Request, IEnumerable<(string Name, string Value)> Headers)
		{
			Request.Headers = Request.Headers == null ? Headers : Request.Headers.Concat(Headers);
			return Request;
		}
		public static Request EnsureSuccess(this Request Request, bool ThrowError)
		{
			Request.EnsureSuccess = ThrowError;
			return Request;
		}
		public static Task<T> Execute<T>(this Request Request, Func<HttpResponseMessage, Task<T>> GetResult)
		{
			return Request.HttpClient.Request(
				Request.GetRequestMessage(),
				async re =>
				{
					if (Request.EnsureSuccess)
						re.EnsureSuccessStatusCode();
					if (Request.OnReturns != null)
						foreach (var r in Request.OnReturns)
							r(re);
					return await GetResult(re);
				}
				);
		}
		public static Task<string> GetString(this Request Request, Encoding Encoding = null)
		{
			return Request.Execute(
				re => re.GetString(Encoding)
				);
		}
		public static Task<byte[]> GetBytes(this Request Request)
		{
			return Request.Execute(
				re => re.GetBytes()
				);
		}
		public static Task Send(this Request Request)
		{
			return Request.Execute(
				re => {
					return Task.FromResult(0);
				}
				);
		}
		//public static Task<T> Get<T>(this IHttpClient client, Uri uri, Func<HttpResponseMessage, Task<T>> GetResult, IEnumerable<(string Name, string Value)> Headers=null)
		//	=> client.Request(new HttpRequestMessage(HttpMethod.Get, uri).WithHeaders(Headers), GetResult);

		//public static Task<T> Post<T>(this IHttpClient client, Uri uri, HttpContent Content,Func<HttpResponseMessage, Task<T>> GetResult, IEnumerable<(string Name, string Value)> Headers = null)
		//	=> client.Request(new HttpRequestMessage(HttpMethod.Post, uri) { Content = Content }.WithHeaders(Headers), GetResult);

		public static async Task<string> GetString(this HttpResponseMessage response, Encoding Encoding)
		{
			if(Encoding==null)
				return await response.Content.ReadAsStringAsync();
			var buf = await response.Content.ReadAsByteArrayAsync();
			return Encoding.GetString(buf);
		}

		public static Task<byte[]> GetBytes(this HttpResponseMessage response)
			=> response.Content.ReadAsByteArrayAsync();

		//public static Task<string> GetString(this IHttpClient client, Uri uri, IEnumerable<(string Name, string Value)> Headers = null)
		//	=> client.Get(uri, r => r.GetString(), Headers);

		//public static Task<byte[]> GetBytes(this IHttpClient client, Uri uri, IEnumerable<(string Name, string Value)> Headers = null)
		//	=> client.Get(uri, r => r.GetBytes(), Headers);

		//public static Task<string> PostAndReturnString(this IHttpClient client, Uri uri, HttpContent Content, IEnumerable<(string Name, string Value)> Headers = null)
		//	=> client.Post(uri,Content,r=>r.GetString(),Headers);

	}
}
