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

using System.Collections.Generic;
using System.Linq;
using SF.Common.Media;
using SF.Common.Media.Storages;
using SF.Sys.Services;
using SF.Sys.Services.Management;
using SF.Sys.Linq;
using SF.Sys.Hosting;

namespace SF.Sys.Services
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
		//public static IServiceInstanceInitializer<IMediaManager> NewMediaManager(
		//	this IServiceInstanceManager manager
		//	)
		//{
		//	return manager.Service<IMediaManager, MediaManager>(new{});
		//}
		//public static IServiceInstanceInitializer NewMediaService(
		//	this IServiceInstanceManager manager,
		//	IServiceInstanceInitializer<IMediaManager> mediaManager = null,
		//	Dictionary<string, string> ExtraStaticResourcePaths = null
		//	)
		//{

		//	if (mediaManager == null)
		//		mediaManager = manager.NewMediaManager();
		//	var stgs = (ExtraStaticResourcePaths?.Select(p =>
		//			 manager.NewStaticFileMediaStorage(p.Key, p.Value) as IServiceInstanceInitializer)
		//			?? Enumerable.Empty<IServiceInstanceInitializer>()
		//		).WithFirst(manager.NewFileSystemMediaStorage("ms"))
		//			.WithFirst(manager.NewStaticFileMediaStorage("ss"))
		//			.WithFirst(mediaManager)
		//			.ToArray();
		//	var svc = manager.DefaultService<IMediaService, MediaService>(
		//		new
		//		{
		//			//Manager=mm.Id,
		//			Setting = new MediaServiceSetting
		//			{
		//				UploadMediaType = "ms"
		//			}
		//		},
		//		stgs
		//		);
		//	return svc;
		//}
		public static IServiceCollection AddMediaService(
			this IServiceCollection sc,
			EnvironmentType EnvType,
			bool InitMediaServiceInstance=true,
			Dictionary<string,string> ExtreStaticResourcePaths=null
			)
		{
			sc.AddSingleton<IMediaMetaCache, MediaMetaCache>();
			sc.AddManagedScoped<IMediaManager, MediaManager>();
			//if(EnvType!=EnvironmentType.Utils)
			sc.AddManagedScoped<IMediaService, MediaService>();

			sc.AddManagedScoped<IMediaStorage, FileSystemMediaStorage>();
			sc.AddManagedScoped<IMediaStorage, StaticFileMediaStorage>();

			if(InitMediaServiceInstance)
				sc.InitServices("媒体服务", async (sp, sim, parent) =>
					{
						await sim.DefaultService<IMediaManager, MediaManager>(new { }).Ensure(sp, parent);

						if(ExtreStaticResourcePaths!=null)
							foreach(var p in ExtreStaticResourcePaths)
								await sim.NewStaticFileMediaStorage(p.Key, p.Value).Ensure(sp, parent);

						await sim.NewFileSystemMediaStorage("ms").Ensure(sp,parent);
						await sim.NewStaticFileMediaStorage("ss").Ensure(sp, parent);

						await sim.DefaultService<IMediaService, MediaService>(
							new
							{
								//Manager=mm.Id,
								Setting = new MediaServiceSetting
								{
									UploadMediaType = "ms"
								}
							}
						).Ensure(sp, parent);
					}
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
