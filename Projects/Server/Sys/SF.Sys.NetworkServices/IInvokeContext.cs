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
using System.Net;

namespace SF.Sys.NetworkService
{
	public interface IInvokeRequest
	{
		string Method { get; }
		string Uri { get; }
		IReadOnlyDictionary<string,IEnumerable<string>> Headers { get; }
		string GetCookie(string Key);
	}
    public static class InvokeRequestExtensions
    {
        public static Uri GetUri(this IInvokeRequest req) =>
            new Uri(req.Uri);
        public static Dictionary<string,string> GetQueryAsDictionary(this IInvokeRequest req) =>
            req.GetUri().ParseQueryToDictionary();
    }
	public interface IInvokeResponse
	{
		string Status { get; set; }
		IDictionary<string, IEnumerable<string>> Headers { get; }
		void SetCookie(Cookie Cookie);
	}
   

    public interface IInvokeContext
	{
		IInvokeRequest Request { get; }
		IInvokeResponse Response { get; }
	}


    
    public interface ILocalInvokeRequest : IInvokeRequest
    {
        new string Method { get; set; }
        new string Uri { get; set; }
        new IDictionary<string, IEnumerable<string>> Headers { get; }
        void SetCookie(Cookie Cookie);
    }
    public interface ILocalInvokeResponse : IInvokeResponse
    {
        Cookie GetCookie(string Key);
    }
    public interface ILocalInvokeContext
    {
        ILocalInvokeRequest Request { get; }
        ILocalInvokeResponse Response { get; }
    }


}
