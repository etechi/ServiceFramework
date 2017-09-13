using SF.Management.FrontEndContents;
using SF.Management.FrontEndContents.Mvc;

namespace SF.Core.ServiceManagement
{
	public static class FrontSideContentsMvcDIExtensions
    {
        public static IServiceCollection AddFrontSideMvcContentRenderProvider(this IServiceCollection sc)
		{
			sc.AddSingleton<IRenderProvider, RazorRender>("razor");
			return sc;
		}
	}
}
