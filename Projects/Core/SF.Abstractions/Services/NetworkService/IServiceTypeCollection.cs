using SF.Metadata;
using SF.Metadata.Models;
using System.Reflection;
namespace SF.Services.NetworkService
{
	public interface IServiceTypeCollection 
	{
		System.Type[] Types { get; }
	}

	public interface IExtraServiceTypeSource
	{
		void AddExtraServiceType(IMetadataBuilder Builder);
	}
}
