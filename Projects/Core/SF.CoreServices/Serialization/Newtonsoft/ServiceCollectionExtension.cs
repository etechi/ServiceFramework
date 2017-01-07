using SF.DI;
using System.Linq;
namespace SF.Serialization
{
	public static class ServiceCollectionExtension
	{
		public static void UseNewtonsoftJson(this IDIServiceCollection sc)
		{
			var s = new Newtonsoft.JsonSerilaizer();
			sc.AddSingleton<IJsonSerializer>(s);
			Json.DefaultSerializer = s;
		}
	}

}
