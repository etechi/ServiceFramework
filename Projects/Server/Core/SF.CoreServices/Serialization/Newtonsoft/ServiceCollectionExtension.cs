using SF.DI;
using System.Linq;
namespace SF.Serialization
{
	public static class ServiceCollectionExtension
	{
		public static void UseNewtonsoftJson(this IDIServiceCollection sc)
		{
			var s = new SF.Serialization.Newtonsoft.JsonSerializer();
			sc.AddSingleton<IJsonSerializer>(s);
		}
	}

}
