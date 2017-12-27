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

using SF.Sys;
using SF.Sys.Caching;
using SF.Sys.Collections.Generic;
using SF.Sys.Drawing;
using SF.Sys.IO;
using SF.Sys.Mime;
using SF.Sys.NetworkService;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SF.Common.Media
{
	/// <summary>
	/// 媒体WebApi配置
	/// </summary>
	public class MediaServiceSetting
	{
		///<title>上传文件类型标识</title>
		/// <summary>
		/// 上传文档ID标识
		/// </summary>
		[Required]
		public string UploadMediaType { get; set; } = "ms";

		///<title>最大文件尺寸(MB)</title>
		/// <summary>
		/// 最大上传文件尺寸(MB)
		/// </summary>
		public int MaxSize { get; set; } = 4;

		///<title>总是转为Jpeg图片</title>
		/// <summary>
		/// 总是转为Jpg图片,节省尺寸。注意png透明色是黑色
		/// </summary>
		public bool ConvertToJpeg { get; set; } = false;

		///<title>图片最大尺寸</title>
		/// <summary>
		/// 上传时会将大图片缩小至该尺寸
		/// </summary>
		public int MaxImageSize { get; set; } = 1920;

		///<title>支持文件类型</title>
		/// <summary>
		/// 支持文件类型
		/// </summary>
		public string[] Mimes { get; set; } = new[]
		{
			"image/jpeg",
			"image/jpg",
			"image/gif",
			"image/png"
		};

		///<title>允许任意图片样式</title>
		/// <summary>
		/// 调试时使用，打开可能会被攻击
		/// </summary>
		public bool SupportedAllFormats { get; set; } = true;

		/// <title>支持图片处理样式</title>
		/// <summary>
		/// 支持图片处理样式
		/// </summary>
		public string[] SupportedFormats { get; set; } = new string[0];

		///<title>缓存的媒体资源类型</title>
		/// <summary>
		/// 不进行缩放时需要缓存的媒体资源类型
		/// </summary>
		public string[] MediaCacheTypes { get; set; }
	}

	/// <summary>
	/// 默认媒体附件服务
	/// </summary>
    public class MediaService : IMediaService
    {
		public MediaServiceSetting Setting { get; }
		public IMediaManager Manager { get; }
		public IMimeResolver MimeResolver { get; }
		public Lazy<IUploadedFileCollection> FileCollection { get; }
		public Lazy<IInvokeContext> InvokeContext { get; }
		public Lazy<IImageProvider> ImageProvider { get; }
		public Lazy<IFileCache> FileCache { get; }



		/// <summary>
		/// 
		/// </summary>
		/// <param name="Manager">媒体管理器</param>
		/// <param name="Setting">服务设置</param>
		/// <param name="MimeResolver">MIME服务</param>
		/// <param name="FileCollection"></param>
		/// <param name="InvokeContext"></param>
		/// <param name="ImageProvider"></param>
		/// <param name="FileCache">文件缓存</param>
		public MediaService(
			IMediaManager Manager,
			MediaServiceSetting Setting,
			IMimeResolver MimeResolver,
			Lazy<IUploadedFileCollection> FileCollection,
			Lazy<IInvokeContext> InvokeContext,
			Lazy<IImageProvider> ImageProvider,
			Lazy<IFileCache> FileCache
			)
		{
			this.Manager = Manager;
			this.Setting = Setting;
			this.MimeResolver = MimeResolver;
			this.FileCollection = FileCollection;
			this.InvokeContext = InvokeContext;
			this.ImageProvider = ImageProvider;
			this.FileCache = FileCache;
		}

		[HeavyMethod]
        public async Task<HttpResponseMessage> Upload( bool returnJson = false)
		{
            try
            {
				var Files = FileCollection.Value.Files;
                if (Files.Length== 0)
                    throw new ArgumentException();
                var file = Files[0];
                if (file.ContentLength > Setting.MaxSize * 1024 * 1024)
                    throw new PublicNotSupportedException($"文件尺寸太大，不能超过{Setting.MaxSize}MB");
                if (!Setting.Mimes.Contains(file.ContentType))
                    throw new PublicNotSupportedException($"不支持这类文件，文件必须是Png,Jpg,Gif等等");

                IMediaMeta mm = null;
                IContent mc = null;
				if (file.ContentType.StartsWith("image/"))
				{
					ImageMemoryBuffer data;
					using (var s = file.OpenStream())
						data = Transforms.Source
						   .ExifOrientationProcess()
						   .WithLimit(Setting.MaxImageSize, Setting.MaxImageSize)
						   .ToMemoryBuffer(Setting.ConvertToJpeg ? "image/jpg" : null)
						   .ApplyTo(s, ImageProvider.Value);
					 mm = new MediaMeta
                    {
                        Height = data.Size.Height,
                        Width = data.Size.Width,
                        Mime = data.Mime,
                        Name = System.IO.Path.GetFileNameWithoutExtension(file.FileName).LetterOrDigits(),
                        Type = Setting.UploadMediaType
					};
                    mc = new Sys.ByteArrayContent
					{
                        Data = data.Data
                    };
                }
                else
                {
                    mm = new MediaMeta
                    {
                        Mime = file.ContentType,
                        Name = System.IO.Path.GetFileNameWithoutExtension(file.FileName).LetterOrDigits(),
                        Type = Setting.UploadMediaType
                    };
					using(var s=file.OpenStream())
						mc = new Sys.ByteArrayContent
						{
							Data = await s.ReadToEndAsync()
						};
                }
                var re = await Manager.SaveAsync(mm, mc);
				if (returnJson)
					return HttpResponse.Json(new
					{
						id=re,
						url = "/r/" + re,
						error = 0
					});
				else
					return HttpResponse.Text(re);
            }
            catch
            {
                if (!returnJson)
                    throw;
				return HttpResponse.Json(new
                {
                    error = 1,
                    message = "上传失败"
                });
            }
		}

		public async Task<string> Clip(string src, double x, double y, double w, double h)
		{
			var m = await Manager.ResolveAsync(src);
			if (m == null)
				throw new PublicArgumentException("找不到指定的文件");

			if (!m.Mime.StartsWith("image/"))
				throw new PublicNotSupportedException("指定的文件不是图片");

			if (x < 0 || y < 0 || w <= 0 || h <= 0 || x + w > 1 || y + h > 1)
				throw new PublicNotSupportedException("参数错误");

			var ctn = await Manager.GetContentAsync(m);

			var img = await Transforms.Source
				.Clip(x, y, w, h)
				.ToMemoryBuffer()
				.ApplyTo(() => ctn.OpenStreamAsync(),ImageProvider.Value);

			var mm = new MediaMeta
			{
				Mime = img.Mime,
				Name = m.Mime,
				Type = Setting.UploadMediaType
			};
			var mc = new Sys.ByteArrayContent
			{
				Data = img.Data
			};
			var re = await Manager.SaveAsync(mm, mc);
			return re;
		}
		int[] ParseFormatArgs(string args)
		{
			return args.Split('x').Select(s => int.Parse(s)).ToArray();
		}
		byte[] FormatImage(byte[] data,string format)
		{
			if (string.IsNullOrEmpty(format)) return data;
			if (format.Length < 2)
				throw new ArgumentException();
			var args = ParseFormatArgs(format.Substring(1));
			if(args.Length>2)
				throw new ArgumentException();

			var w = args[0];
			var h = args.Length == 2 ? args[1] : w;
			switch (format[0])
			{
				case 'c':
					return Transforms.Source.Clip(w, h).ToByteArray().ApplyTo(data, ImageProvider.Value);
				case 's':
					return Transforms.Source.WithLimit(w, h).ToByteArray().ApplyTo(data, ImageProvider.Value);
				default:
					throw new NotSupportedException();
			}
		}

		async Task<FileCacheContent> LoadContent(string id,string format)
		{
			var m = await Manager.ResolveAsync(id);
			if (m == null) return null;
			var data = await Manager.GetContentAsync(m);
			return new FileCacheContent
			{
				FileExtension = MimeResolver.MimeToFileExtension(m.Mime),
				Content = FormatImage(await data.GetByteArrayAsync(), format)
			};
		}
		async Task<HttpResponseMessage> GetStorageContent(string id)
		{
			var m = await Manager.ResolveAsync(id);
			if (m == null)
				return null;
			var data = await Manager.GetContentAsync(m);
			if (data == null)
				return null;
			if (data is IFileContent)
				return HttpResponse.File(((IFileContent)data).FilePath, data.ContentType);
			return HttpResponse.ByteArray(
				await data.GetByteArrayAsync(),
				data.ContentType
				);
		}

		async Task<HttpResponseMessage> GetCachedContent(string id,string format)
		{
			var file_name = (id + "-" + format).UTF8Bytes().CalcHash(Hash.MD5()).Hex();

			var path = await FileCache.Value.Cache(
				file_name,
				() => LoadContent(id, format)
				);
			if (path == null)
				return null;

			return HttpResponse.File(path,MimeResolver.FileExtensionToMime(Path.GetExtension(path)));
		}

		public async Task<HttpResponseMessage> Get(string id, string format=null)
		{
			if (format == null)
				format = "";
			else if (!Setting.SupportedAllFormats &&
				!Setting.SupportedFormats.Contains(format))
				return HttpResponse.NotFound;

			var etag = "\""+id + ":" + format+"\"";
			var Request = InvokeContext.Value.Request;
			if (Request.Headers.Get("If-None-Match")?.SingleOrDefault()?.Contains(etag) ?? false)
				return HttpResponse.NotModified;


			var type_end = id.IndexOf('-');
			if (type_end == -1)
				return HttpResponse.NotFound;
			var type = id.Substring(0, type_end);
			var msg =
				string.IsNullOrEmpty(format) &&
				!(Setting.MediaCacheTypes?.Contains(type) ?? false) ?
				await GetStorageContent(id) :
				await GetCachedContent(id, format);
			if (msg == null)
				return HttpResponse.NotFound;

			msg.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
			{
				MaxAge = TimeSpan.FromDays(30),
				Public = true,
			};
			msg.Headers.ETag = new System.Net.Http.Headers.EntityTagHeaderValue(etag);
			return msg;
		}

	}
}
