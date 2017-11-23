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
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using SF.Sys;

namespace SF.Common.FrontEndContents.Mvc
{
	public abstract class BaseRender : IRenderProvider
	{
		protected abstract void OnRender(HtmlHelper htmlHelper, string view,string config, IContent content,object data);

		public void Render(object rawRenderContext, string view, string config, IContent content,object data)
		{
			OnRender((HtmlHelper)rawRenderContext, view, config, content, data);
		}
	}
	public class RenderModel
	{
		public RenderModel(IContent content, object data, IDictionary<string, string> Args)
		{
			this.Content = content;
			this.Data = data;
			this.Args = Args;
		}
		public IContent Content { get; }
		public object Data { get; }
		public IDictionary<string,string> Args { get; }
	}
	public class RenderModel<T> : RenderModel
	{
		public RenderModel(IContent content,object data, IDictionary<string, string> Args) :base(content, data,Args)
		{
		}
		public new T Data { get { return (T)base.Data; }  }

	}

    public class RazorRender : BaseRender
	{
		static System.Collections.Concurrent.ConcurrentDictionary<Type, Func<IContent, object, IDictionary<string, string>,object>> creators = new System.Collections.Concurrent.ConcurrentDictionary<Type, Func<IContent, object, IDictionary<string, string>, object>>();
		static object CreateRenderModel(IContent content,object data,IDictionary<string,string> args)
		{
			var type = data == null ? typeof(object) : data.GetType();
			Func<IContent, object, IDictionary<string, string>,object> creator;
			if(!creators.TryGetValue(type,out creator))
			{
				var arg_content = Expression.Parameter(typeof(IContent), "content");
				var arg_data = Expression.Parameter(typeof(object), "data");
				var arg_args = Expression.Parameter(typeof(IDictionary<string, string>), "args");
				creator = Expression.Lambda<Func<IContent, object, IDictionary<string, string>,object>>(
					Expression.New(
						typeof(RenderModel<>).MakeGenericType(type).GetConstructor(
							System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.CreateInstance,
							null,
							new[] { typeof(IContent), typeof(object),typeof(IDictionary<string, string>) },
							null
							),
						arg_content,
						arg_data,
						arg_args
					),
					arg_content,
					arg_data,
					arg_args
					).Compile();
				creator = creators.GetOrAdd(type, creator);
			}
			return creator(content, data, args);
		}
		protected override void OnRender(HtmlHelper htmlHelper, string view, string config,IContent content,object data)
		{
			IDictionary<string, string> args = null;
			if (!string.IsNullOrWhiteSpace(config))
				args = Json.Parse<Dictionary<string, string>>(config);
			htmlHelper.RenderPartial(view, CreateRenderModel(content, data, args));
		}
	}
}
