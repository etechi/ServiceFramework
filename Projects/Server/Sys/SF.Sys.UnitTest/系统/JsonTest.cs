using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SF.Sys;
using SF.Sys.UnitTest;

namespace SF.UT.系统
{
	[TestClass]
	public class JsonTest : TestBase
	{
		class Message
		{
			public string Type { get; set; }
			public object Value { get; set; }
		}
		class Payload
		{
			public int Value1 { get; set; }
			public int Value2 { get; set; }
			public int Value3 { get; set; }
		}
		[TestMethod]
		public void 反序列化()
		{
			var msg = new Message
			{
				Type = "test",
				Value = new Payload
				{
					Value1 = 1,
					Value2 = 2,
					Value3 = 4
				}
			};
			var str = Json.Stringify(msg);

			var re = Json.Parse<Message>(str);
			var pl = ((JObject)re.Value).ToObject<Payload>();
			Assert.IsTrue(Poco.DeepEquals(msg.Value, pl));
		}

		class Value
		{
			public Value()
			{
			}
			public string S { get; set; } = "a";
		}
		[TestMethod]
		public void 反序列化_默认值()
		{
			var setting = new Newtonsoft.Json.JsonSerializerSettings { DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore };
			Assert.AreEqual("a", Newtonsoft.Json.JsonConvert.DeserializeObject<Value>("{}", setting).S);
			Assert.AreEqual("x", Newtonsoft.Json.JsonConvert.DeserializeObject<Value>("{S:\"x\"}", setting).S);
			Assert.AreEqual("a", Newtonsoft.Json.JsonConvert.DeserializeObject<Value>("{S:null}", setting).S);

			Assert.AreEqual("{}", Newtonsoft.Json.JsonConvert.SerializeObject(new Value { S = null }, setting));
			Assert.AreEqual("{\"S\":\"x\"}", Newtonsoft.Json.JsonConvert.SerializeObject(new Value { S = "x"}, setting));
			Assert.AreEqual("{\"S\":\"a\"}", Newtonsoft.Json.JsonConvert.SerializeObject(new Value { S = "a" }, setting));


			//var setting1 = new Newtonsoft.Json.JsonSerializerSettings { DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include };
			//Assert.AreEqual("a", Newtonsoft.Json.JsonConvert.DeserializeObject<Value>("{}", setting1).S);
			//Assert.AreEqual("x", Newtonsoft.Json.JsonConvert.DeserializeObject<Value>("{S:\"x\"}", setting1).S);
			//Assert.AreEqual("a", Newtonsoft.Json.JsonConvert.DeserializeObject<Value>("{S:null}", setting1).S);

			//Assert.AreEqual("{\"S\":null}", Newtonsoft.Json.JsonConvert.SerializeObject(new Value { S = null }, setting1));
			//Assert.AreEqual("{\"S\":\"x\"}", Newtonsoft.Json.JsonConvert.SerializeObject(new Value { S = "x" }, setting1));
			//Assert.AreEqual("{\"S\":\"a\"}", Newtonsoft.Json.JsonConvert.SerializeObject(new Value { S = "a" }, setting1));

		}
	}


}
