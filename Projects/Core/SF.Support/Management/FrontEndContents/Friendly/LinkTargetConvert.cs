
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndContents.Friendly
{
	public static class LinkTargetConvert
	{
		public static LinkTarget ToFriendly(string target)
		{
			return string.IsNullOrWhiteSpace(target) ? LinkTarget._default :
				target == "_self" ? LinkTarget._self :
				LinkTarget._blank;
		}
		public static string FromFriendly(LinkTarget target)
		{
			return target == LinkTarget._default ? null : target == LinkTarget._self ? "_self" : "_blank";
		}
	}
	
}