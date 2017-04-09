using System;
using Newtonsoft.Json;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace SF.Core.ManagedServices.Runtime
{
	class ServiceCreateParameterTemplate : IServiceCreateParameterTemplate,ManagedServices.IServiceInstanceMeta
	{
		public IReadOnlyList<object> Args { get; private set; }
		public IReadOnlyDictionary<string, string> ServiceIdents { get; private set; }
		public string ServiceInstanceIdent { get; private set; }
		public ManagedServices.IServiceInstanceMeta GetServiceInstanceIdent() => this;
		string ManagedServices.IServiceInstanceMeta.Ident => ServiceInstanceIdent;

		long IServiceInstanceMeta.AppId => throw new NotImplementedException();

		long IServiceInstanceMeta.FeatureId => throw new NotImplementedException();

		class ServiceConverter : JsonConverter
		{
			public IServiceDetector ServiceDetector { get; set; }
			public Dictionary<string, string> ServiceIdents { get; } = new Dictionary<string, string>();
			public override bool CanConvert(Type objectType)
			{
				return ServiceDetector.IsServiceType(objectType);
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
		class ArrayToDictionaryConverter : JsonConverter
		{
			abstract class DictDeserializer
			{
				public abstract object Deserialize(JsonReader reader, JsonSerializer serializer, out string[] Keys);
			}
			class DictDeserializer<K,T> : DictDeserializer
			{
				Func<T,K> GetIdentFunc { get; }
				public DictDeserializer(PropertyInfo prop)
				{
					var arg = Expression.Parameter(typeof(T), "item");
					GetIdentFunc = Expression.Lambda<Func<T, K>>(
						Expression.Property(arg, prop),
						arg
						).Compile();
				}
				public override object Deserialize(JsonReader reader, JsonSerializer serializer,out string[] Keys)
				{
					var keyList = new List<string>();
					var re=serializer.Deserialize<T[]>(reader).ToDictionary(i =>
					{
						var key = GetIdentFunc(i);
						keyList.Add(key.ToString());
						return key;
					});
					Keys = keyList.ToArray();
					return re;
				}
			}
			static System.Collections.Concurrent.ConcurrentDictionary<Type, DictDeserializer> DictDeserializers { get; } = new System.Collections.Concurrent.ConcurrentDictionary<Type, ArrayToDictionaryConverter.DictDeserializer>();
			static DictDeserializer DictDeserializerCreator(Type itype)
			{
				if (!itype.IsGeneric())
					return null;
				if (itype.GetGenericTypeDefinition() != typeof(Dictionary<,>))
					return null;
				var gtypes = itype.GetGenericArguments();
				var keyFields = gtypes[1]
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
					.Where(p => p.IsDefined(typeof(KeyAttribute)))
					.ToArray();
				if (keyFields.Length != 1)
					return null;
				if (keyFields[0].PropertyType != gtypes[0])
					return null;
				return (DictDeserializer)Activator.CreateInstance(typeof(DictDeserializer<,>).MakeGenericType(
					gtypes[0],
					gtypes[1]),
					keyFields[0]
					);
			}
			static Func<Type, DictDeserializer> DictDeserializerCreatorFunc { get; } = DictDeserializerCreator;
			static DictDeserializer GetDictDeserializer(Type type)
			{
				return DictDeserializers.GetOrAdd(type, DictDeserializerCreatorFunc);
			}
			public Dictionary<string,string> ReplacePath(Dictionary<string, string> dict)
			{
				if (KeyMap == null)
					return dict;
				// a.b[1].c.d[3].e
				return dict.ToDictionary(p => {
					var key = p.Key;
					var i = key.Length-1;
					for (;;)
					{
						var bb = key.LastIndexOf('[', i);
						if (bb == -1)
							break;
						var be = key.IndexOf(']', bb + 1);
						if (be == -1)
							break;
						var prefix = key.Substring(0, bb);
						var keys = KeyMap.Get(prefix);
						if (keys != null)
							key = key.Substring(0, bb) + "[" + keys[int.Parse(key.Substring(bb + 1, be - bb - 1))]+"]"+key.Substring(be+1);
						i = bb - 1;
					}
					return key;
				}, p => p.Value);
			}
			public override bool CanConvert(Type objectType)
			{
				return GetDictDeserializer(objectType) != null;
			}
			Dictionary<string, string[]> KeyMap;
			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				if (reader.TokenType != JsonToken.StartArray)
					return serializer.Deserialize(reader, objectType);
				//a.b[  1].c => a.b.key.c
				string[] keys;
				var path = reader.Path;
				var re= GetDictDeserializer(objectType).Deserialize(reader, serializer,out keys);
				if (KeyMap == null) KeyMap = new Dictionary<string, string[]>();
				KeyMap.Add(path, keys);
				return re;
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}
		}


		static bool SkipComment(JsonTextReader jr)
		{
			for (;;)
			{
				if (!jr.Read())
					return false;
				if (jr.TokenType == JsonToken.Comment)
					continue;
				return true;
			}
		}
		static void ReadToObjectStart(JsonTextReader jr)
		{
			if (!SkipComment(jr))
				throw new InvalidOperationException("服务配置数据格式错误");
			if (jr.TokenType != JsonToken.StartObject)
				throw new InvalidOperationException("服务配置必须是一个JSON对象");
		}
		static string ReadPropertyName(JsonTextReader jr)
		{
			if (!SkipComment(jr))
				throw new InvalidOperationException("服务配置数据格式错误");
			if (jr.TokenType == JsonToken.EndObject)
				return null;
			if (jr.TokenType != JsonToken.PropertyName)
				throw new InvalidOperationException("服务配置必须是一个JSON对象");
			return (string)jr.Value;
		}
		static void Skip(JsonTextReader jr)
		{
			SkipComment(jr);
			if (jr.TokenType == JsonToken.StartArray || jr.TokenType == JsonToken.StartConstructor || jr.TokenType == JsonToken.StartObject)
				jr.Skip();
		}
		class ServiceIdent : ManagedServices.IServiceInstanceMeta
		{
			public string Ident { get; set; }

			public long AppId { get; set; }

			public long FeatureId { get; set; }
		}
		public static ServiceCreateParameterTemplate Load(
			ConstructorInfo ci,
			string ServiceInstanceId,
			string Config,
			IServiceDetector ServiceDetector
			)
		{
			object[] args;
			var parameters = ci.GetParameters();
			IReadOnlyDictionary<string, string> sis;
			if (string.IsNullOrWhiteSpace(Config) || Config.Trim()=="null")
			{
				sis = new Dictionary<string, string>();
				args = new object[parameters.Length];
			}
			else
			{
				var atod = new ArrayToDictionaryConverter();
				var sc = new ServiceConverter()
				{
					ServiceDetector = ServiceDetector,
				};
				var setting = new JsonSerializerSettings
				{
					Converters = new JsonConverter[]
					{
						sc,
						atod
					}
				};
				using (var jr = new JsonTextReader(new System.IO.StringReader(Config)))
				{
					ReadToObjectStart(jr);
					var pdic = parameters.ToDictionary(p => p.Name);
					var serializer = JsonSerializer.Create(setting);
					var adic = new Dictionary<string, object>();
					for (;;)
					{
						var prop = ReadPropertyName(jr);
						if (prop == null)
							break;
						ParameterInfo p;
						if (pdic.TryGetValue(prop, out p))
						{
							var realType = p.ParameterType.GetGenericArgumentTypeAsLazy() ??
								p.ParameterType.GetGenericArgumentTypeAsFunc() ??
								p.ParameterType;

							var st = realType.IsInterfaceType() ? ServiceDetector.GetServiceType(realType) : ServiceType.Unknown;
							if (st == ServiceType.Normal)
							{
								Skip(jr);
								continue;
							}
							if (st == ServiceType.Managed)
							{
								SkipComment(jr);
								sc.ServiceIdents.Add(prop, (string)jr.Value);
								continue;
							}
							SkipComment(jr);
							adic[prop] = serializer.Deserialize(jr, p.ParameterType);
						}
						else
						{
							Skip(jr);
						}
					}
					
					args = parameters.Select(p => {
						object a;
						adic.TryGetValue(p.Name, out a);
						return a;
					}).ToArray();
					sis = atod.ReplacePath(sc.ServiceIdents);
				}
			}

			return new ServiceCreateParameterTemplate
			{
				Args = args,
				ServiceIdents = sis,
				ServiceInstanceIdent= ServiceInstanceId
			};
		}

		public object GetArgument(int Index)
		{
			return Args[Index];
		}

		public string GetServiceIdent(string Path)
		{
			string id;
			ServiceIdents.TryGetValue(Path, out id);
			return id;
		}
	}
}
