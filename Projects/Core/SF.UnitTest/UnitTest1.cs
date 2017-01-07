using System;
using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using SF.Reflection;
namespace SF.UnitTest
{
    public class UnitTest1
    {
		class FixedContractResolver : DefaultContractResolver
		{
			protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
			{
				var p=base.CreateProperty(member, memberSerialization);
				if (member is PropertyInfo pi && pi.PropertyType.IsEnumType())
					p.DefaultValue = Enum.ToObject(pi.PropertyType, -1);
				return p;
			}
		}
		enum TestEnum
		{
			V1,
			V2,
			V3
		}
        [Fact]
        public void Test1()
        {
			var setting = new JsonSerializerSettings
			{
				ContractResolver = new FixedContractResolver(),
				DefaultValueHandling=DefaultValueHandling.IgnoreAndPopulate,
				Converters = new[]
				{
					new Newtonsoft.Json.Converters.StringEnumConverter()
				}
			};

			var re=JsonConvert.SerializeObject(new
			{
				a=TestEnum.V1,
				b=TestEnum.V2,
				c=12,
				d="!212"
			}, setting);
			 re = JsonConvert.SerializeObject(new
			{
				a = TestEnum.V1,
				b = TestEnum.V2,
				c = 12,
				d = "!212"
			}, setting);
		}


		public interface IService1
		{

		}
		public interface IService2
		{

		}
		public class CfgItem
		{
			public IService2 Svc2 { get; set; }
			public int V1 { get; set; }
		}
		public class CfgTest
		{
			public string Str1 { get; set; }
			public IService1 Svc1 { get; set; }
			public IEnumerable<CfgItem> Items { get; set; }
		}

		class ServiceConverter : JsonConverter
		{
			public Dictionary<string, string> ServiceIdents { get; } = new Dictionary<string, string>();
			public override bool CanConvert(Type objectType)
			{
				var ti = objectType.GetTypeInfo();
				return ti.IsInterface && (!ti.IsGenericType || ti.GetGenericTypeDefinition() != typeof(IEnumerable<>));
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				ServiceIdents.Add(reader.Path, (string)reader.Value);
				return null;
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}
		}
		[Fact]
		public void Test2()
		{
			var setting = new JsonSerializerSettings
			{
				Converters = new[]
				{
					new ServiceConverter()
				}
			};

			var re = JsonConvert.DeserializeObject< CfgTest>(
				"{\"str1\":\"aaa\",\"svc1\":\"s1\",\"Items\":[{\"svc2\":\"s2\",\"V1\":12},{\"svc2\":\"s3\",\"V1\":13}]}",
				setting);
		}

	}
}
