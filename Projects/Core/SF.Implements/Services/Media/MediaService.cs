using SF.Core;
using SF.Core.Caching;
using SF.Metadata;
using SF.Services.Drawing;
using SF.Services.NetworkService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Media
{
	[Comment(Name = "媒体WebApi配置")]
	public class MediaServiceSetting
	{
		[Comment(Name = "上传文件类型标识", Description = "上传文档ID标识")]
		[Required]
		public string UploadMediaType { get; set; } = "ms";

		[Comment(Name = "最大文件尺寸(MB)", Description = "最大上传文件尺寸(MB)")]
		public int MaxSize { get; set; } = 4;

		[Comment(Name = "总是转为Jpeg图片", Description = "总是转为Jpg图片,节省尺寸。注意png透明色是黑色")]
		public bool ConvertToJpeg { get; set; } = false;

		[Comment(Name = "图片最大尺寸", Description = "上传时会将大图片缩小至该尺寸")]
		public int MaxImageSize { get; set; } = 1920;

		[Comment(Name = "支持文件类型", Description = "支持文件类型")]
		public string[] Mimes { get; set; } = new[]
		{
			"image/jpeg",
			"image/jpg",
			"image/gif",
			"image/png"
		};

		[Comment(Name = "允许任意图片样式", Description = "调试时使用，打开可能会被攻击")]
		public bool SupportedAllFormats { get; set; } = true;

		[Comment(Name = "支持图片处理样式", Description = "支持图片处理样式")]
		public string[] SupportedFormats { get; set; } = new string[0];

		[Comment(Name = "图片缓存目录", Description = "图片缓存目录，以~开头的目录相对于网站根目录,默认为~/App_Data/temp/cache")]
		[Required]
		public string CacheRoot { get; set; } = "~/App_Data/tmp/cache";

		[Comment(Name = "缓存的媒体资源类型", Description = "不进行缩放时需要缓存的媒体资源类型")]
		public string[] MediaCacheTypes { get; set; }
	}

	[Comment(Name = "默认媒体附件服务", GroupName = "系统服务")]
    public abstract class MediaService : IMediaService
    {
		public MediaServiceSetting Setting { get; }
		public IMediaManager Manager { get; }
		public KB.Mime.IMimeResolver MimeResolver { get; }
		public Lazy<IUploadedFileCollection> FileCollection { get; }
		public Lazy<IHttpRequestSource > HttpRequestSource { get; }
		public Lazy<IImageProvider> ImageProvider { get; }
		public Lazy<IFileCache> FileCache { get; }
		public MediaService(
			[Comment("媒体管理器")]
			IMediaManager Manager,
			[Comment("服务设置")]
			MediaServiceSetting Setting,
			[Comment("MIME服务")]
			KB.Mime.IMimeResolver MimeResolver,
			Lazy<IUploadedFileCollection> FileCollection,
			Lazy<IHttpRequestSource> HttpRequestSource,
			Lazy<IImageProvider> ImageProvider,
			Lazy<IFileCache> FileCache
			)
		{
			this.Manager = Manager;
			this.Setting = Setting;
			this.MimeResolver = MimeResolver;
			this.FileCollection = FileCollection;
			this.HttpRequestSource = HttpRequestSource;
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
                    var data = Transforms.Source
                        .ExifOrientationProcess()
                        .WithLimit(Setting.MaxImageSize, Setting.MaxImageSize)
                        .ToMemoryBuffer(Setting.ConvertToJpeg ? "image/jpg" : null)
                        .ApplyTo(file.InputStream, ImageProvider.Value);
                    mm = new MediaMeta
                    {
                        Height = data.Size.Height,
                        Width = data.Size.Width,
                        Mime = data.Mime,
                        Name = System.IO.Path.GetFileNameWithoutExtension(file.FileName).LetterOrDigits(),
                        Type = Setting.UploadMediaType
					};
                    mc = new ByteArrayContent
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
                    mc = new ByteArrayContent
                    {
                        Data = await file.InputStream.ReadToEndAsync()
                    };
                }
                var re = await Manager.SaveAsync(mm, mc);
				if (returnJson)
					return Http.Json(new
					{
						url = "/r/" + re,
						error = 0
					});
				else
					return Http.Text(re);
            }
            catch
            {
                if (!returnJson)
                    throw;
				return Http.Json(new
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
			var mc = new ByteArrayContent
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

		async Task<KeyValuePair<string,byte[]>> LoadContent(string id,string format)
		{
			var m = await Manager.ResolveAsync(id);
			if (m == null) return new KeyValuePair<string, byte[]>(null, null);
			var data = await Manager.GetContentAsync(m);
			return new KeyValuePair<string, byte[]>(
				MimeResolver.MimeToFileExtension(m.Mime),
				FormatImage(await data.GetByteArrayAsync(),format)
				);

		}
		static System.Threading.SemaphoreSlim CatchLocker { get; } = new System.Threading.SemaphoreSlim(1);
		async Task<HttpResponseMessage> GetStorageContent(string id)
		{
			var m = await Manager.ResolveAsync(id);
			if (m == null)
				return null;
			var data = await Manager.GetContentAsync(m);
			if (data == null)
				return null;
			if (data is IFileContent)
				return Http.File(((IFileContent)data).Path, data.ContentType);

			return Http.ByteArray(
				await data.GetByteArrayAsync(),
				data.ContentType
				);
		}

		async Task<HttpResponseMessage> GetCachedContent(string id,string format)
		{
			var file_name = Hash.MD5((id + "-" + format).UTF8Bytes()).Hex();

			var path = await FileCache.Value.Cache(
				file_name,
				CatchLocker,
				() => LoadContent(id, format)
				);
			if (path == null)
				return null;

			return Http.File(path,MimeResolver.FileExtensionToMime(Path.GetExtension(path)));
		}

		public async Task<HttpResponseMessage> Get(string id, string format=null)
		{
			if (format == null)
				format = "";
			else if (!Setting.SupportedAllFormats &&
				!Setting.SupportedFormats.Contains(format))
				return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
			var etag = "\""+id + ":" + format+"\"";
			var Request = HttpRequestSource.Value.Request;
			if (Request.Headers.IfNoneMatch?.FirstOrDefault()?.Tag == etag)
				return new HttpResponseMessage(System.Net.HttpStatusCode.NotModified);


			var type_end = id.IndexOf('-');
			if (type_end == -1)
				return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
			var type = id.Substring(0, type_end);
			var msg =
				string.IsNullOrEmpty(format) &&
				!(Setting.MediaCacheTypes?.Contains(type) ?? false) ?
				await GetStorageContent(id) :
				await GetCachedContent(id, format);
			if (msg == null)
				return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);

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
