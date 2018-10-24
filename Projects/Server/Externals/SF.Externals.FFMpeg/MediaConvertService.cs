using SF.Common.Media;
using SF.Common.Notifications.Senders;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Caching;
using SF.Sys.Collections.Generic;
using SF.Sys.Hosting;
using SF.Sys.HttpClients;
using SF.Sys.Linq;
using SF.Sys.Mime;
using SF.Sys.NetworkService;
using SF.Sys.Reflection;
using SF.Sys.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Externals.FFMpeg
{
    public class MediaConvertService : IMediaConvertService
    {
        static string FFMpegPath { get; }
        IFileCache FileCache { get; }
        IMimeResolver MimeResolver { get; }

        Lazy<IMediaManager> MediaManager { get; }
        Lazy<IFilePathResolver> FilePathResolver { get; }
        string GetFFMpegPath()
        {
            var path=FilePathResolver.Value.Resolve("config://ffmpeg/ffmpeg.exe");
            if (path != null && System.IO.File.Exists(path))
                return path;
            var assPath = System.Reflection.Assembly.GetExecutingAssembly().GetBasePath();
            path = System.IO.Path.Combine(assPath, "ffmpeg", "ffmpeg.exe");
            if (!System.IO.File.Exists(path))
                throw new PublicArgumentException("找不到ffmpeg");
            return path;
        }
        public MediaConvertService(IFileCache FileCache, IMimeResolver MimeResolver, Lazy<IMediaManager> MediaManager,Lazy<IFilePathResolver> FilePathResolver)
        {
            this.FileCache = FileCache;
            this.MimeResolver = MimeResolver;
            this.MediaManager = MediaManager;
            this.FilePathResolver = FilePathResolver;
        }
        public async Task FileConvert(string input,string output)
        {
            using (var p = new Process())
            {
                p.StartInfo.FileName = GetFFMpegPath();
                p.StartInfo.Arguments = $"-y -i \"{input}\" \"{output}\"";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = false;
                //p.StartInfo.RedirectStandardOutput = true;

                var tcs = new TaskCompletionSource<int>();
                p.Exited += new EventHandler((o, e) =>
                {
                    if (p.ExitCode != 0)
                        tcs.TrySetException(new PublicInvalidOperationException("转换失败"));
                    else
                        tcs.SetResult(0);
                });
                p.EnableRaisingEvents = true;
                p.Start();
                

                await tcs.Task;
            }
        }
        async Task LoadContent(string id, string dst, string format)
        {
            /*http://ffmpeg.zeranoe.com/builds/win64/static/
            download first
            ffmpeg - latest - win64 -static.7z file
            ffmpeg - i file.amr file.mp3
            */
            var output = dst;

            var m = await MediaManager.Value.ResolveAsync(id);
            if (m == null)
                throw new PublicArgumentException("找不到媒体:"+id);
            var data = (await MediaManager.Value.GetContentAsync(m)) as FileContent;
            if(data==null)
                throw new PublicArgumentException("找不到媒体文件");
            var input = data.FilePath;
            await FileConvert(input, output);
        }

        public async Task<HttpResponseMessage> Convert(string id, string target)
        {
            if (target != "mp3")
                throw new PublicNotSupportedException("不支持指定的格式:" + target);

            var file_name = (id + "-" + target).UTF8Bytes().CalcHash(Hash.MD5()).Hex();
            var path = await FileCache.Cache(
                file_name+"."+target,
                dst => LoadContent(id,dst,target)
                );
            if (path == null)
                return null;
            return HttpResponse.File(path, MimeResolver.FileExtensionToMime("."+target));

        }
    }

}
