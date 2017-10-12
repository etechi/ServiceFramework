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
using System.Reflection;

namespace SF.Core.ServiceManagement
{
	//public interface IServiceDescriptor
	//{
	//	IServiceDescriptor Comment(string Name, string Title = null, string SubTitle = null, string Description = null, string Remarks = null);
	//	IServiceDescriptor Ident(string Ident);
	//	IServiceDescriptor Child(IServiceDescriptor ServiceDescriptor);
	//}
	//public interface IServiceBuilder
	//{
	//	IServiceDescriptor Service<I, T>(object Setting = null);
	//}


	public static class MediaDIServiceCollectionExtension
	{
		//public static IServiceDescriptor DeclareMediaService(this IServiceCollection sc)
		//{
		//	var sb = (IServiceBuilder)sc.First(sd => sd.ServiceImplementType == ServiceImplementType.Instance && sd.InterfaceType == typeof(IServiceBuilder));
		//	return sb.Service<IMediaService, MediaService>(new
		//	{
		//		//Manager=mm.Id,
		//		//FileCache=fc,
		//		Setting = new MediaServiceSetting
		//		{
		//			UploadMediaType = "ms"
		//		}
		//	}).Child(
		//			sb.Service<IMediaManager, MediaManager>(
		//			new
		//			{
		//				Setting = new
		//				{
		//					Types = new[]
		//					{
		//								new
		//								{
		//									Type="ms",
		//									Storage=sb.Service<IMediaStorage, FileSystemMediaStorage>(
		//										new
		//										{
		//											//PathResolver = fpr,
		//											RootPath = "data://media/default"
		//										}
		//										)
		//								},
		//								new
		//								{
		//									Type="ss",
		//									Storage=sb.Service<IMediaStorage, StaticFileMediaStorage>(
		//										new
		//										{
		//											//PathResolver = fpr,
		//											RootPath = "data://meida/static"
		//										}
		//										)
		//								}
		//					}
		//				}
		//			}
		//			)
		//		);

		//}
		


		public static IServiceInstanceInitializer<IMediaStorage> NewStaticFileMediaStorage(
			this IServiceInstanceManager manager,
			string Ident,
			string rootPath=null
			)
		{
			return manager.ServiceWithIdent<IMediaStorage, StaticFileMediaStorage>(
				Ident,
				new
				{
					//PathResolver = fpr,
					RootPath = rootPath??"data://meida/static"
				}
				).WithIdent(Ident);
		}
		public static IServiceInstanceInitializer<IMediaStorage> NewFileSystemMediaStorage(
			this IServiceInstanceManager manager,
			string Ident
			)
		{
			return manager.ServiceWithIdent<IMediaStorage, FileSystemMediaStorage>(
				Ident,
				new
				{
					//PathResolver = fpr,
					RootPath = "data://media/default"
				}
				).WithIdent(Ident);
		}
		public static IServiceInstanceInitializer<IMediaManager> NewMediaManager(
			this IServiceInstanceManager manager
			)
		{
			return manager.Service<IMediaManager, MediaManager>(new{});
		}
		public static IServiceInstanceInitializer NewMediaService(
			this IServiceInstanceManager manager,
			IServiceInstanceInitializer<IMediaManager> mediaManager = null,
			Dictionary<string, string> ExtraStaticResourcePaths = null
			)
		{

			if (mediaManager == null)
				mediaManager = manager.NewMediaManager();
			var stgs = (ExtraStaticResourcePaths?.Select(p =>
					 manager.NewStaticFileMediaStorage(p.Key, p.Value) as IServiceInstanceInitializer)
					?? Enumerable.Empty<IServiceInstanceInitializer>()
				).WithFirst(manager.NewFileSystemMediaStorage("ms"))
					.WithFirst(manager.NewStaticFileMediaStorage("ss"))
					.WithFirst(mediaManager)
					.ToArray();
			var svc = manager.DefaultService<IMediaService, MediaService>(
				new
				{
					//Manager=mm.Id,
					Setting = new MediaServiceSetting
					{
						UploadMediaType = "ms"
					}
				},
				stgs
				);
			return svc;
		}
		public static IServiceCollection AddMediaService(
			this IServiceCollection sc,
			EnvironmentType EnvType,
			bool InitMediaServiceInstance=true,
			Dictionary<string,string> ExtreStaticResourcePaths=null
			)
		{
			sc.AddSingleton<IMediaMetaCache, MediaMetaCache>();
			sc.AddManagedScoped<IMediaManager, MediaManager>();
			if(EnvType!=EnvironmentType.Utils)
				sc.AddManagedScoped<IMediaService, MediaService>();

			sc.AddManagedScoped<IMediaStorage, FileSystemMediaStorage>();
			sc.AddManagedScoped<IMediaStorage, StaticFileMediaStorage>();

			if(InitMediaServiceInstance)
				sc.InitService("媒体服务", (sp, sim) =>
					sim.NewMediaService(null, ExtreStaticResourcePaths)
					);


			//sc.AddInitializer(
			//	"初始化媒体服务组",
			//	async sp =>
			//	{
			//		var sim = sp.Resolve<IServiceInstanceManager>();

			//		//var fpr = await sim.ResolveDefaultService<IFilePathResolver>();
			//		//var fc = await sim.ResolveDefaultService<IFileCache>();
			//		var ms=await sim.TryAddService<IMediaStorage, FileSystemMediaStorage>(
			//			null,
			//			new
			//			{
			//				//PathResolver = fpr,
			//				RootPath = "data://media/default"
			//			});
			//		var ss=await sim.TryAddService<IMediaStorage, StaticFileMediaStorage>(
			//			null,
			//			new
			//			{
			//				//PathResolver = fpr,
			//				RootPath = "data://meida/static"
			//			});
			//		var mm=await sim.EnsureDefaultService<IMediaManager, MediaManager>(
			//			null,
			//			new
			//			{
			//				Setting=new 
			//				{
			//					Types=new[]
			//					{
			//						new {
			//							Type="ms",
			//							Storage=ms.Id
			//						},
			//						new
			//						{
			//							Type="ss",
			//							Storage=ss.Id
			//						}
			//					} 
			//				}
			//			});
			//		await sim.SetServiceParent(ms.Id, mm.Id);
			//		await sim.SetServiceParent(ss.Id, mm.Id);
			//		var svc=await sim.EnsureDefaultService<IMediaService, MediaService>(
			//			null,
			//			new
			//			{
			//				//Manager=mm.Id,
			//				//FileCache=fc,
			//				Setting=new MediaServiceSetting
			//				{
			//					UploadMediaType="ms"
			//				}
			//			});
			//		await sim.SetServiceParent(mm.Id, svc.Id);

			//	});

			return sc;
		}
	}
}
