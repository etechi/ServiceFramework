using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Media.StaticFiles
{
	class FileMediaMeta : IMediaMeta
	{
		public string Path { get; set; }
		public int Height{get;set;}
		public string Id{get;set;}
		public int Length{get;set;}
		public string Mime{get;set;}
		public string Name{get;set;}
		public string Type{get;set;}
		public int Width{get;set;}
	}
	
	public class MediaStorage : IMediaStorage
	{
		public string RootPath { get; }
		public string IDPrefix { get; }
		public KB.Mime.IMimeResolver MimeResolver { get; }
		public MediaStorage(string IDPrefix,string RootPath, KB.Mime.IMimeResolver MimeResolver)
		{
			this.IDPrefix = IDPrefix;
			if (RootPath[RootPath.Length - 1] == '\\' || RootPath[RootPath.Length - 1] == '/')
				this.RootPath = RootPath.Substring(0, RootPath.Length - 1);
			else
				this.RootPath = RootPath;

			this.MimeResolver = MimeResolver;
		}
		public Task<bool> RemoveAsync(string Id)
        {
            throw new NotSupportedException();
        }
		public Task<IMediaMeta> ResolveAsync(string Id)
		{
			if (Id[0]=='.' || Id[0]=='/' || Id[0]=='\\' || Id.Contains(':'))
				throw new NotSupportedException();
			var lid = Id.LastIndexOf('-');
			if (lid == -1)
				return null;
			var file= Id.Substring(0, lid).Replace('-', '\\') + "." + Id.Substring(lid + 1);

			var path = System.IO.Path.Combine(RootPath, file);
			if (!System.IO.File.Exists(path))
				return Task.FromResult((IMediaMeta)null);
			var mime = MimeResolver.FileExtensionToMime(System.IO.Path.GetExtension(path));
			return Task.FromResult(
				(IMediaMeta)new FileMediaMeta
				{
					Id = Id,
					Type = IDPrefix,
					Mime = mime,
					Name = System.IO.Path.GetFileNameWithoutExtension(path),
					Path = path
				});
		}

		public Task<IContent> GetContentAsync(IMediaMeta Media)
		{
			var fm = Media as FileMediaMeta;
			if (fm == null)
				throw new NotSupportedException();
			return Task.FromResult(
				(IContent)new FileContent { Path = fm.Path}
				);
		}

		public Task<string> SaveAsync(IMediaMeta media, IContent Content)
		{
			throw new NotSupportedException();
		}
	}
}
