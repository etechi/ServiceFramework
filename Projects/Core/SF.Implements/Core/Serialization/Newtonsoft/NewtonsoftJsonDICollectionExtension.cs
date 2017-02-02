using SF.Core.Serialization;
using SF.Core.Serialization.Newtonsoft;
using System.Linq;
namespace SF.Core.DI
{
	public static class NewtonsoftJsonDICollectionExtension
	{
		public static void UseNewtonsoftJson(this IDIServiceCollection sc)
		{
			var s = new JsonSerializer();
			sc.AddSingleton<IJsonSerializer>(s);
			Json.DefaultSerializer = s;
		}
	}

}
