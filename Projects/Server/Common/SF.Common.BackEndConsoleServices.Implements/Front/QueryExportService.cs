using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities;
using SF.Sys.Services;
using SF.Sys.Collections.Generic;

namespace SF.Sys.BackEndConsole.Front
{
	public class QueryExportService : IQueryExportService
	{
		IServiceMetadata ServiceMetadata { get; set; }
		public QueryExportService(
			IServiceMetadata ServiceMetadata
			)
		{
			this.ServiceMetadata = ServiceMetadata;
		}

		class ExportContext
		{
			public string Format { get; set; }
			public string Argument { get; set; }
			public string Title { get; set; }
			public bool PostMode { get; set; }
			public Controller Controller { get; set; }
			public Models.Metadata.Action Action { get; set; }
		}

		IEnumerable<string> GetServiceNames(string Name)
		{
			yield return Name;
			yield return "I" + Name;
			yield return "I" + Name+"Service";
		}

		public async Task<HttpResponseMessage> Export(
			string Service, 
			string Method, 
			string Format, 
			string Argument, 
			string Title = null
			)
		{

			var desc=GetServiceNames(Service).Select(
				s => ServiceMetadata.ServicesByTypeName.Get(s)
				).Where(d => d != null).FirstOrDefault();




			var controller = Library.Controllers.Where(c => c.Name == Controller).SingleOrDefault();
			UIEnsure.NotNull(controller, "找不到控制器:" + Controller);

			var action = controller.Actions.Where(a => a.Name == Action).SingleOrDefault();
			UIEnsure.NotNull(action, "找不到动作:" + action);

			string entity = null;
			var ema = controller.Attributes.FirstOrDefault(a => a.Type == typeof(ServiceProtocol.Annotations.EntityManagerAttribute).FullName);
			if (ema != null)
			{
				var values = ServiceProtocol.Json.Decode<Dictionary<string, string>>(ema.Values);
				entity = values.Get("Entity");
			}


			if (Title == null)
			{
				var em = controller.GetSysType().GetCustomAttribute<EntityManagerAttribute>(true) as EntityManagerAttribute;
				Title = (em?.Title) ?? (em?.Entity) ?? controller.Name;
			}
		

			var method = action.GetSysMethod();
			var PostMode = action.HttpMethods.Contains("Post");
			var ctx = new ExportContext
			{
				Title = Title,
				Controller = controller,
				Action = action,
				Argument = PostMode ? Argument :
					UriExtension.EncodeQueryString(
						Json.Parse<Dictionary<string, object>>(Argument)
						),
				PostMode = PostMode,
				Format = Format
			};

			var retType = method.ReturnType;
			if (retType.IsGenericType &&
				retType.GetGenericTypeDefinition() == typeof(Task<>)
				)
				retType = retType.GetGenericArguments()[0];

			if (retType.IsGenericType &&
				retType.GetGenericTypeDefinition() == typeof(ServiceProtocol.ObjectManager.QueryResult<>)
				)
			{
				return await ExportQueryResult(ctx, retType.GetGenericArguments()[0]);
			}
			if (retType.IsArray)
				return await ExportArray(ctx, retType.GetElementType());

			return await ExportObject(ctx, retType);
		}


		async Task<object> Query(
			ExportContext ectx,
			string QueryString,
			string PostData
			)
		{
			var uri = $"http://localhost/{ectx.Controller.Name}/{ectx.Action.Name}";
			if (QueryString != null)
				uri += "?" + QueryString;

			var rd = new HttpRouteData(EmptyRoute);
			rd.Values["controller"] = ectx.Controller.Name;
			rd.Values["action"] = ectx.Action.Name;

			var ctx = new HttpRequestContext
			{
				RouteData = rd
			};
			using (var dispatcher = new ServiceProtocol.Web.Http.ControllerDispatcher(
				System.Web.Http.GlobalConfiguration.Configuration
				))
			{
				var req = new HttpRequestMessage();
				req.Method = ectx.PostMode ? HttpMethod.Post : HttpMethod.Get;
				req.RequestUri = new Uri(uri);
				if (ectx.PostMode)
					req.Content = new StringContent(PostData, Encoding.UTF8, "application/json");
				return await dispatcher.GetActionResultAsync(req, ctx);
			}
		}
		class Exporter : IDisposable
		{
			ITableExporter exporter;
			public string FileExtension { get; }
			MemoryStream stream { get; }
			public Exporter(
				IDIProviderResolver<ITableExporterFactory> Resolver,
				ExportContext ctx,
				Column[] columns
				)
			{
				stream = new MemoryStream();
				this.exporter = Resolver.Resolve(ctx.Format).Create(
					stream,
					ctx.Title,
					columns
					);
				ContentType = exporter.ContentType;
				FileExtension = exporter.FileExtension;
			}
			public void AddRow(object[] rows)
			{
				exporter.AddRow(rows);
			}
			public byte[] GetBytes()
			{
				if (exporter != null)
				{
					exporter.Dispose();
					exporter = null;
				}
				return stream.ToArray();
			}
			public string ContentType { get; }
			public void Dispose()
			{
				if (exporter != null)
					exporter.Dispose();
				if (stream != null)
					stream.Dispose();
			}
		}
		Exporter CreateExporter(ExportContext ctx, Column[] columns)
		{
			return new Exporter(ProviderResolver, ctx, columns);
		}
		void Serialize<T>(ExportContext ctx, StringBuilder sb, T data)
		{
			sb.AppendLine(ServiceProtocol.Json.Encode(data));
		}
		IEnumerable<Type> AllTypes(Type type)
		{
			var l = new List<Type>();
			while (type != null)
			{
				l.Add(type);
				type = type.BaseType;
			}
			l.Reverse();
			return l;
		}
		IEnumerable<PropertyInfo> AllProperties(Type type)
		{
			foreach (var t in AllTypes(type))
				foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
					yield return p;
		}
		async Task<IHttpActionResult> ExportQueryResultTyped<T>(ExportContext ctx, Type type)
		{
			var sb = new StringBuilder();
			var pageCount = 1000;
			var query = (ctx.PostMode || string.IsNullOrEmpty(ctx.Argument) ? "" : ctx.Argument + "&") + "_pl=" + pageCount;

			var re = (ObjectManager.QueryResult<T>)await Query(ctx, query, ctx.PostMode ? ctx.Argument : null);
			var total = re.Total;
			var offset = 0;
			var props = (from prop in type.GetProperties()
						 let tablevisible = prop.GetCustomAttribute<TableVisibleAttribute>(true) as TableVisibleAttribute
						 where tablevisible != null
						 orderby tablevisible.Order
						 let display = prop.GetCustomAttribute<DisplayAttribute>(true) as DisplayAttribute
						 select new
						 {
							 col = new Column
							 {
								 Name = display == null ? prop.Name : display.Name,
								 Type = prop.PropertyType
							 },
							 prop = prop
						 }
						).ToArray();


			using (var exporter = CreateExporter(ctx, props.Select(p => p.col).ToArray()))
			{
				for (; ; )
				{
					var count = 0;
					foreach (var item in re.Items)
					{
						exporter.AddRow(props.Select(p => p.prop.GetValue(item)).ToArray());
						count++;
					}
					if (count < pageCount)
						break;
					offset += count;
					query = (ctx.PostMode || string.IsNullOrEmpty(ctx.Argument) ? "" : ctx.Argument + "&") + "_pl=" + pageCount + "&_po=" + offset;
					re = (ObjectManager.QueryResult<T>)await Query(ctx, query, ctx.PostMode ? ctx.Argument : null);
				}

				return Content(
					exporter.GetBytes(),
					exporter.ContentType,
					ctx.Title + DateTime.Now.ToString("-yyyyMMddHHmm") + "." + exporter.FileExtension);
			}

		}

		MethodInfo GetMethod(string Name, Type type)
		{
			var method = GetType()
				.GetMethods(BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(m => m.Name == Name)
				.SingleOrDefault();
			return method.MakeGenericMethod(type);
		}

		Task<IHttpActionResult> ExportQueryResult(ExportContext ctx, Type type)
		{
			var method = GetMethod("ExportQueryResultTyped", type);
			return (Task<IHttpActionResult>)method.Invoke(this, new object[] { ctx, type });
		}

		async Task<IHttpActionResult> ExportArrayTyped<T>(ExportContext ctx, Type type)
		{
			var sb = new StringBuilder();
			var re = (T[])await Query(ctx, ctx.PostMode ? null : ctx.Argument, ctx.PostMode ? ctx.Argument : null);
			var props = (from prop in type.GetProperties()
						 let tablevisible = prop.GetCustomAttribute<TableVisibleAttribute>(true) as TableVisibleAttribute
						 where tablevisible != null
						 orderby tablevisible.Order
						 let display = prop.GetCustomAttribute<DisplayAttribute>(true) as DisplayAttribute
						 select new
						 {
							 col = new Column
							 {
								 Name = display == null ? prop.Name : display.Name,
								 Type = prop.PropertyType
							 },
							 prop = prop
						 }
						).ToArray();


			using (var exporter = CreateExporter(ctx, props.Select(p => p.col).ToArray()))
			{
				var count = 0;
				foreach (var item in re)
				{
					exporter.AddRow(props.Select(p => p.prop.GetValue(item)).ToArray());
					count++;
				}

				return Content(
					exporter.GetBytes(),
					exporter.ContentType,
					ctx.Title + DateTime.Now.ToString("-yyyyMMddHHmm") + "." + exporter.FileExtension);
			}
		}
		Task<IHttpActionResult> ExportArray(ExportContext ctx, Type type)
		{
			var method = GetMethod("ExportArrayTyped", type);
			return (Task<IHttpActionResult>)method.Invoke(this, new object[] { ctx, type });
		}
		async Task<IHttpActionResult> ExportObjectTyped<T>(ExportContext ctx, Type type)
		{
			var re = (T)await Query(ctx, ctx.PostMode ? null : ctx.Argument, ctx.PostMode ? ctx.Argument : null);

			var sb = new StringBuilder();
			Serialize<T>(ctx, sb, re);
			return Content(sb.ToString(), "text/plain");
		}
		Task<IHttpActionResult> ExportObject(ExportContext ctx, Type type)
		{
			var method = GetMethod("ExportObjectTyped", type);
			return (Task<IHttpActionResult>)method.Invoke(this, new object[] { ctx, type });
		}
	}
}
