﻿using SF.Core;
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
using SF.Core.ManagedServices;
using SF.Core.ManagedServices.Admin;
using SF.Core.Hosting;

namespace SF.Core.DI
{
	public static class MediaDIServiceCollectionExtension
	{
		public static IDIServiceCollection UseMediaService(
			this IDIServiceCollection sc
			)
		{
			sc.Normal().AddSingleton<IMediaMetaCache, MediaMetaCache>();
			sc.AddScoped<IMediaManager, MediaManager>();
			sc.AddScoped<IMediaService, MediaService>();

			sc.AddScoped<IMediaStorage, SF.Services.Media.Storages.FileSystemMediaStorage>();
			sc.AddScoped<IMediaStorage, SF.Services.Media.Storages.StaticFileMediaStorage>();

			if(sc.IsManagedServiceCollection())
			sc.AddInitializer(
				"初始化媒体服务组",
				async sp =>
				{
					var sim = sp.Resolve<ManagedServices.Admin.IServiceInstanceManager>();

					var fpr = await sim.ResolveDefaultService<IFilePathResolver>();
					var fc = await sim.ResolveDefaultService<IFileCache>();
					var ms=await sim.TryAddService<IMediaStorage, Services.Media.Storages.FileSystemMediaStorage>(
						new
						{
							PathResolver = fpr,
							RootPath = "data://media/default"
						});
					var ss=await sim.TryAddService<IMediaStorage, Services.Media.Storages.StaticFileMediaStorage>(
						new
						{
							PathResolver = fpr,
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
					await sim.EnsureDefaultService<IMediaService, MediaService>(
						new
						{
							Manager=mm.Id,
							FileCache=fc,
							Setting=new MediaServiceSetting
							{
								UploadMediaType="ms"
							}
						});
				});

			return sc;
		}
	}
}