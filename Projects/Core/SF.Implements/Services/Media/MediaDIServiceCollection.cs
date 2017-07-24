using SF.Core;
using SF.Core.Caching;
using SF.Metadata;
using SF.Core.Drawing;
using SF.Core.NetworkService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SF.Services.Media;
using SF.Core.Hosting;
using SF.Core.ServiceManagement.Management;
using SF.Services.Media.Storages;

namespace SF.Core.ServiceManagement
{
	public interface IServiceDescriptor
	{
		IServiceDescriptor Comment(string Name, string Title = null, string SubTitle = null, string Description = null, string Remarks = null);
		IServiceDescriptor Ident(string Ident);
		IServiceDescriptor Child(IServiceDescriptor ServiceDescriptor);
	}
	public interface IServiceBuilder
	{
		IServiceDescriptor Service<I, T>(object Setting = null);
	}
	public static class MediaDIServiceCollectionExtension
	{
		public static IServiceDescriptor DeclareMediaService(this IServiceCollection sc)
		{
			var sb = (IServiceBuilder)sc.First(sd => sd.ServiceImplementType == ServiceImplementType.Instance && sd.InterfaceType == typeof(IServiceBuilder));
			return sb.Service<IMediaService, MediaService>(new
			{
				//Manager=mm.Id,
				//FileCache=fc,
				Setting = new MediaServiceSetting
				{
					UploadMediaType = "ms"
				}
			}).Child(
					sb.Service<IMediaManager, MediaManager>(
					new
					{
						Setting = new
						{
							Types = new[]
							{
										new
										{
											Type="ms",
											Storage=sb.Service<IMediaStorage, FileSystemMediaStorage>(
												new
												{
													//PathResolver = fpr,
													RootPath = "data://media/default"
												}
												)
										},
										new
										{
											Type="ss",
											Storage=sb.Service<IMediaStorage, StaticFileMediaStorage>(
												new
												{
													//PathResolver = fpr,
													RootPath = "data://meida/static"
												}
												)
										}
							}
						}
					}
					)
				);

		}
		public static IServiceCollection UseMediaService(
			this IServiceCollection sc,
			EnvironmentType EnvType
			)
		{
			sc.AddSingleton<IMediaMetaCache, MediaMetaCache>();
			sc.AddScoped<IMediaManager, MediaManager>();
			if(EnvType!=EnvironmentType.Utils)
				sc.AddScoped<IMediaService, MediaService>();

			sc.AddScoped<IMediaStorage, FileSystemMediaStorage>();
			sc.AddScoped<IMediaStorage, StaticFileMediaStorage>();

			sc.AddInitializer(
				"初始化媒体服务组",
				async sp =>
				{
					var sim = sp.Resolve<IServiceInstanceManager>();

					//var fpr = await sim.ResolveDefaultService<IFilePathResolver>();
					//var fc = await sim.ResolveDefaultService<IFileCache>();
					var ms=await sim.TryAddService<IMediaStorage, FileSystemMediaStorage>(
						new
						{
							//PathResolver = fpr,
							RootPath = "data://media/default"
						});
					var ss=await sim.TryAddService<IMediaStorage, StaticFileMediaStorage>(
						new
						{
							//PathResolver = fpr,
							RootPath = "data://meida/static"
						});
					var mm=await sim.EnsureDefaultService<IMediaManager, MediaManager>(
						new
						{
							Setting=new 
							{
								Types=new[]
								{
									new {
										Type="ms",
										Storage=ms.Id
									},
									new
									{
										Type="ss",
										Storage=ss.Id
									}
								} 
							}
						});
					await sim.SetServiceParent(ms.Id, mm.Id);
					await sim.SetServiceParent(ss.Id, mm.Id);
					var svc=await sim.EnsureDefaultService<IMediaService, MediaService>(
						new
						{
							//Manager=mm.Id,
							//FileCache=fc,
							Setting=new MediaServiceSetting
							{
								UploadMediaType="ms"
							}
						});
					await sim.SetServiceParent(mm.Id, svc.Id);

				});

			return sc;
		}
	}
}
