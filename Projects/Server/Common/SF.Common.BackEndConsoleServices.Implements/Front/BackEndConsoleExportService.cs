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
using SF.Sys.Reflection;
using SF.Utils.TableExports;
using SF.Sys.Comments;
using SF.Sys.Linq.Expressions;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Clients;

namespace SF.Sys.BackEndConsole.Front
{
	
	public class BackEndConsoleExportService : IBackEndConsoleExportService
	{
		IServiceMetadata ServiceMetadata { get; }
		IServiceInvokerProvider ServiceInvokerProvider { get; }
		IServiceProvider ServiceProvider { get; }
		IAuthService AuthService { get; }
		IAccessToken AccessToken { get; }
		NamedServiceResolver<ITableExporterFactory> ExporterFactoryResolver { get; }
		IAccessTokenValidator AccessTokenValidator { get; }
		SF.Sys.NetworkService.Metadata.Library NetLibrary { get; }
		public BackEndConsoleExportService(
			IServiceProvider ServiceProvider,
			IServiceMetadata ServiceMetadata,
			IServiceInvokerProvider ServiceInvokerProvider,
			IAuthService AuthService,
			IAccessToken AccessToken,
			IAccessTokenValidator AccessTokenValidator,
			SF.Sys.NetworkService.Metadata.Library NetLibrary,
			NamedServiceResolver<ITableExporterFactory> ExporterFactoryResolver
			)
		{
			this.NetLibrary = NetLibrary;
			this.AccessTokenValidator = AccessTokenValidator;
			this.AccessToken = AccessToken;
			this.AuthService = AuthService;
			this.ServiceProvider = ServiceProvider;
			this.ServiceMetadata = ServiceMetadata;
			this.ServiceInvokerProvider = ServiceInvokerProvider;
			this.ExporterFactoryResolver = ExporterFactoryResolver;
		}

		class ExportContext
		{
			public ExportMode Mode { get; set; }
			/// <summary>
			/// 服务
			/// </summary>
			[Required]
			public string Service { get; set; }
			/// <summary>
			/// 方法
			/// </summary>
			[Required]
			public string Method { get; set; }
			/// <summary>
			/// 格式
			/// </summary>
			[Required]
			public string Format { get; set; }
			/// <summary>
			/// 参数
			/// </summary>
			public string Argument { get; set; }
			/// <summary>
			/// 标题
			/// </summary>
			public string Title { get; set; }
			public IServiceInvoker Invoker { get; set; }
		}

		IEnumerable<string> GetServiceNames(string Name)
		{
			yield return Name;
			yield return "I" + Name;
			yield return "I" + Name+"Service";
		}

		public async Task<HttpResponseMessage> Export(
			ExportMode Mode,
			string Service,
			string Method,
			string Format,
			string Argument,
			string Title,
			string Token
			)
		{

			var (svcType, mthd) = NetLibrary.FindMethod(Service, Method);
			if (svcType == null || mthd == null)
				throw new PublicArgumentException($"找不到服务{Service}或方法{Method}");

			var invoker = ServiceInvokerProvider.Resolve(svcType.GetSysType(), mthd.GetSysMethod());
			var user = Token != null ? await AccessTokenValidator.Validate(Token) : AccessToken.User;
			if (user==null || !user.Identity.IsAuthenticated || !AuthService.Authorize(
				user, 
				invoker.ServiceDeclaration.ServiceName, 
				invoker.Method.Name, 
				null))
				throw new PublicDeniedException("您无权访问此接口");

			var ctx = new ExportContext
			{
				Method=Method,
				Format=Format,
				Mode=Mode,
				Service=Service,
				Title=Title,
				Argument=Argument,
				Invoker=invoker
			};
			switch (Mode)
			{
				case ExportMode.Table:
					return await ExportQueryResult(ctx);
				default:
					throw new NotSupportedException("不支持指定的类型:" + Mode);
			}
		}
		

		class Exporter : IDisposable
		{
			ITableExporter exporter;
			public string FileExtension { get; }
			MemoryStream stream { get; }
			public Exporter(
				NamedServiceResolver<ITableExporterFactory> Resolver,
				ExportContext ctx,
				Column[] columns
				)
			{
				stream = new MemoryStream();
				this.exporter = Resolver(ctx.Format).Create(
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
			return new Exporter(ExporterFactoryResolver, ctx, columns);
		}

		MethodInfo ExportMethodInfo { get; } = 
			typeof(BackEndConsoleExportService).GetMethodExt(
				nameof(ExportQueryResultTyped),
				BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,				
				typeof(ExportContext), 
				typeof(Type),
				typeof(bool)
				).IsNotNull();
		Task<HttpResponseMessage> ExportQueryResult(ExportContext ctx)
		{
			var invoker = ctx.Invoker;
			var args = invoker.Method.GetParameters();
			if (args.Length != 1 || !typeof(IPagingArgument).IsAssignableFrom(args[0].ParameterType))
				throw new PublicNotSupportedException("接口方法不支持导出,参数必须为QueryArgument的子类");

			var resultType = invoker.Method.ReturnType.GetGenericArgumentTypeAsTask() ?? invoker.Method.ReturnType;
			Type resultItemType;
			bool isEnumerable = false;
			if (resultType.IsGeneric() && resultType.GetGenericTypeDefinition() == typeof(QueryResult<>))
				resultItemType = resultType.GetGenericArguments()[0];
			else
			{
				resultItemType = resultType.AllInterfaces().Select(i => i.GetGenericArgumentTypeAsEnumerable()).FirstOrDefault(i => i != null);
				isEnumerable = true;
			}
			if (resultItemType==null)
				throw new PublicNotSupportedException("接口方法不支持导出,返回类型不是QueryResult<>或IEnumerable<>类型");

			return (Task < HttpResponseMessage > )ExportMethodInfo.MakeGenericMethod(resultItemType).Invoke(
				this,
				new object[] {
				ctx,
				resultItemType,
				isEnumerable
				}
				);
		}
		async Task<HttpResponseMessage> ExportQueryResultTyped<T>(ExportContext ctx, Type type,bool isEnumerable)
		{

			var parameter = ctx.Invoker.Method.GetParameters()[0];
			var arg = (IPagingArgument)Json.DefaultSerializer.Deserialize(
				ctx.Argument, 
				parameter.ParameterType
				);


			var title = ctx.Title = ctx.Title ?? type.Comment().Title;

			var props = (
				from t in ADT.Link.ToEnumerable(type, t => t.BaseType).Reverse()
				from prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty).Where(p=>p.DeclaringType==t)
					let tablevisible = prop.GetCustomAttribute<TableVisibleAttribute>(true) as TableVisibleAttribute
					where tablevisible != null
					orderby tablevisible.Order
					select new
					{
						col = new Column
						{
							Name = prop.Comment().Title ?? prop.Name,
							Type = prop.PropertyType
						},
						prop = prop
					}
				).ToArray();


			using (var exporter = CreateExporter(ctx, props.Select(p => p.col).ToArray()))
			{
				if (isEnumerable)
				{
					var re = (IEnumerable<T>)await ctx.Invoker.InvokeAsync(ServiceProvider, new[] { arg });
					foreach (var item in re)
						exporter.AddRow(props.Select(p => p.prop.GetValue(item)).ToArray());
				}
				else
				{
					var pageCount = 1000;

					var offset = 0;

					for (; ; )
					{

						arg.Paging = new Paging
						{
							Count = pageCount,
							Offset = offset
						};
						var re = (QueryResult<T>)await ctx.Invoker.InvokeAsync(ServiceProvider, new[] { arg });

						var count = 0;
						foreach (var item in re.Items)
						{
							exporter.AddRow(props.Select(p => p.prop.GetValue(item)).ToArray());
							count++;
						}
						if (count < pageCount)
							break;
						offset += count;
					}
				}
				var ctn = new System.Net.Http.ByteArrayContent(
						exporter.GetBytes()
						);
				ctn.Headers.ContentType=new System.Net.Http.Headers.MediaTypeHeaderValue(exporter.ContentType);
				ctn.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
				{
					FileName = title + DateTime.Now.ToString("-yyyyMMddHHmm") + "." + exporter.FileExtension
				};
				return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
				{
					Content = ctn
				};	
			}

		}
	}
}
