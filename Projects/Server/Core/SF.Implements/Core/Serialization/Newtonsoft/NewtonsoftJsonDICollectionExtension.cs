using SF.Core.Serialization;
using SF.Core.Serialization.Newtonsoft;
using System.Linq;
namespace SF.Core.ServiceManagement
{
	public static class NewtonsoftJsonDICollectionExtension
	{
		public static void AddNewtonsoftJson(this IServiceCollection sc)
		{
			var s = new JsonSerializer();
			sc.AddSingleton<IJsonSerializer>(s);
			Json.DefaultSerializer = s;
		}
	}

}