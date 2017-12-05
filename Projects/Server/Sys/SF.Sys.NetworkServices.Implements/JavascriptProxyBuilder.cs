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

using SF.Sys.Linq;
using System;
using System.Linq;
using System.Text;

namespace SF.Sys.NetworkService
{

	public class JavascriptProxyBuilder
    {

        Func<Metadata.Service, Metadata.Method, bool> ActionFilter { get; }

        StringBuilder sb{get;} = new StringBuilder();
		string ApiName { get; }
		public JavascriptProxyBuilder(string Namespace, Func<Metadata.Service, Metadata.Method, bool> ActionFilter)
        {
			this.ApiName = Namespace;
            this.ActionFilter = ActionFilter;
        }
		void BuildMethod(Metadata.Service service, Metadata.Method method)
		{
			sb.AppendLine($"{method.Name}:[{(method.HeavyMode?"true":"false")},{(method.HeavyMode && (method?.Parameters?.Length??0)==1?"null":$"[{method?.Parameters?.Select(p=>"\""+p.Name+"\"").Join(",")}]")}],");
		}
		void BuildService(Metadata.Service service)
		{
            var methods = service.Methods.Where(a => ActionFilter(service,a)).ToArray();
            if (methods.Length == 0)
                return;
			sb.AppendLine($"{service.Name}:{{");
			foreach (var a in methods)
			{
				BuildMethod(service, a);
			}
			sb.AppendLine("},");
		}

		public string Build(Metadata.Library Library)
		{
			sb.AppendLine(@"
var _invoker=null;
function buildArguments(args,params){
	if(!params)return args[0];
	var re={};
	var c=Math.min(args.length,params.length);
	for(var i=0;i<c;i++)
		re[params[i]]=args[i];
	return re;
}
function buildFunc(svc,method,post,params){
	return function(){
		var args=buildArguments(arguments,params);
		return _invoker(svc,method,post,args);
	}
}

function build(svcs){
var re={};
for(var sn in svcs){
	var svc=svcs[sn];
	var sd=re[sn]={};
	for(var mn in svc){
		var m=svc[m];
		sd[mn]=buildFunc(sn,mn,m[0],m[1]);
	}
}
return re;
}

export function setServiceInvoker(invoker){
	_invoker=invoker;
}
");

			sb.AppendLine($"export const {ApiName}=build({{");
			foreach (var c in Library.Services)
				BuildService(c);
			sb.AppendLine("});");
			return sb.ToString();
		}
	}

}
