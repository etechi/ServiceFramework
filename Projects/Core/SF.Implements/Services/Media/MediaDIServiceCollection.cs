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
namespace SF.Core.DI
{
	public static class MediaDIServiceCollectionExtension
	{
		public static IDIServiceCollection UseMediaService(this IDIServiceCollection sc)
		{
			sc.AddSingleton<IMediaMetaCache, MediaMetaCache>();
			sc.AddScoped<IMediaManager, MediaManager>();
			sc.AddScoped<IMediaService, MediaService>();

			sc.AddScoped<IMediaStorage, SF.Services.Media.Storages.FileSystemMediaStorage>();
			sc.AddScoped<IMediaStorage, SF.Services.Media.Storages.StaticFileMediaStorage>();

			return sc;
		}
	}
}
