using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SF.Core.Logging;
using SF.Core.ServiceManagement;
using SF.Services.FrontEndContents;
using SF.Services.FrontEndContents.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
