using System.Threading.Tasks;
using SF.Common.Media;
using SF.Common.Notifications.Senders;
using SF.Externals.FFMpeg;
using SF.Sys.NetworkService;

namespace SF.Sys.Services
{
	
	public static class FFMPegDIExtension
    {
        public static IServiceCollection AddFFMpegMediaConvertServices(this IServiceCollection sc)
        {
            sc.AddScoped<IMediaConvertService, MediaConvertService>();
			return sc;
		}
        
    }
}
