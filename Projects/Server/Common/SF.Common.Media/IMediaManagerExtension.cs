using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Common.Media
{
	public static class IMediaManagerExtension
	{
		public static async Task<string> CreateByUri(
			this IMediaManager manager,
			string Type,
			Uri uri,
			int MaxSize,
			Action<System.Net.Http.HttpContent> validator = null
			)
		{
			using (var cli = new System.Net.Http.HttpClient())
			{
				using (var res = await cli.GetAsync(uri))
				{
					if (validator != null)
						validator(res.Content);

					if ((res.Content.Headers.ContentLength ?? 0) > MaxSize)
						throw new ArgumentException("资源太大，无法保存");

					byte[] data;
					if (res.Content.Headers.ContentLength.HasValue)
						data = await res.Content.ReadAsByteArrayAsync();
					else
					{
						var buf = new byte[4096];
						var readed = 0;
						using (var ms = new System.IO.MemoryStream())
						{
							using (var s = await res.Content.ReadAsStreamAsync())
							{
								for (; ; )
								{
									var re = await s.ReadAsync(buf, 0, buf.Length);
									if (re == 0)
										break;
									readed += re;
									if (readed > MaxSize)
										throw new ArgumentException("资源太大，无法保存");
									ms.Write(buf, 0, re);
								}
								data = ms.ToArray();
							}
						}
					}

					return await manager.SaveAsync(
						new MediaMeta
						{
							Mime = res.Content.Headers.ContentType.MediaType,
							Type = Type
						},
						new SF.Sys.ByteArrayContent
						{
							Data = data

						}
					);
				}
			}
		}
		public static async Task<string> CreateByImageUri(
			this IMediaManager manager,
			string Type,
			Uri uri,
			int MaxSize,
			Action<System.Net.Http.HttpContent> validator = null
			)
		{
			return await manager.CreateByUri(Type, uri, MaxSize, ctn =>
			{
				if (!ctn.Headers.ContentType.MediaType.StartsWith("image/"))
					throw new ArgumentException("提供的地址不是图片地址");
				if (validator != null)
					validator(ctn);
			});
		}
		public static async Task<string> TryCreateByImageUri(
			this IMediaManager manager,
			string Type,
			string uri,
			int MaxSize
			)
		{
			if (string.IsNullOrWhiteSpace(uri))
				return null;
			Uri u;
			if (!Uri.TryCreate(uri, UriKind.Absolute, out u))
				throw new ArgumentException("URL不合法：" + uri);

			return await manager.CreateByImageUri(Type, u, MaxSize);
		}

		public static async Task<SF.Sys.IContent> LoadContent(this IMediaManager mm, string id)
		{
			var res = await mm.ResolveAsync(id);
			if (res == null) return null;
			return await mm.GetContentAsync(res);
		}
		public static async Task<byte[]> LoadBytes(this IMediaManager mm, string id)
		{
			var ctn = await mm.LoadContent(id);
			if (ctn == null) return null;
			return await ctn.GetByteArrayAsync();
		}
	}
}
