using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SF.Entities
{
	public static class EntityIdents
	{
		static char[] EncodeChars { get; } = new char[] { '~', '-', '/', '.', ':' };
		static bool[] EncodeCharBools { get; } = EncodeChars.Aggregate(new bool[256], (s, c) => { s[(int)c] = true; return s; });
		static string[] EncodeCharEscaped { get; } =
			EncodeChars
			.Select((c, i) => (c, i))
			.Aggregate(new string[256], (s, c) => { s[(int)c.c] = ((char)('a' + c.i)).ToString(); return s; });

		static Func<char, string> CharEscaper { get; } = c => EncodeCharEscaped[(int)c];
		static Func<string, int, (int, char)> CharUnescaper { get; } = (s, i) => (1, EncodeChars[(int)s[i] - 'a']);


		public static void EncodePart(string part, int offset, int length, StringBuilder buffer)
		{
			part.Escape(offset, length, EncodeCharBools, CharEscaper, buffer);
		}
		public static void DecodePart(string part, int offset, int length, StringBuilder buffer)
		{
			part.Unescape(offset, length, '~', CharUnescaper, buffer);
		}

		public static string Build(string Type, params string[] Parts)
			=> Build(Type, (IEnumerable<string>)Parts);
		public static string Build(string Type, IEnumerable<string> Parts)
		{
			var sb = new StringBuilder();
			EncodePart(Type, 0, Type.Length, sb);
			if (Parts != null)
				foreach (var Part in Parts)
				{
					sb.Append('-');
					EncodePart(Part, 0, Part.Length, sb);
				}
			return sb.ToString();
		}
		public static string Parse(string id, int begin, int length, out string[] Parts)
		{
			var end = begin + length;
			Parts = null;
			var i = id.IndexOf('-', begin, end - begin);
			if (i == -1)
				return null;
			var sb = new StringBuilder();
			DecodePart(id, begin, i, sb);
			var type = sb.ToString();


			var partList = new List<string>();
			i++;
			for (; ; )
			{
				var t = id.IndexOf('-', i, end - i);
				sb.Length = 0;
				DecodePart(id, i, t == -1 ? end : t, sb);
				partList.Add(sb.ToString());
				if (t == -1)
					break;
				i = t + 1;
			}
			Parts = partList.ToArray();
			return type;
		}


		public static CommonEntityIdent Parse(string Ident)
		{
			var type = Parse(Ident, 0, Ident.Length, out var keys);
			return new CommonEntityIdent(type, keys);
		}
		public static EntityIdent<K1> Parse<K1>(string Ident)
			where K1:IEquatable<K1>
		{
			var type = Parse(Ident, 0, Ident.Length, out var keys);
			if (keys.Length != 1)
				throw new ArgumentException("对象ID包含主键字段个数错误:" + Ident);
			return new EntityIdent<K1>(type, (K1)Convert.ChangeType(keys[0], typeof(K1)));
		}
		public static EntityIdent<K1,K2> Parse<K1,K2>(string Ident)
			where K1 : IEquatable<K1>
			where K2 : IEquatable<K2>
		{
			var type = Parse(Ident, 0, Ident.Length, out var keys);
			if (keys.Length != 2)
				throw new ArgumentException("对象ID包含主键字段个数错误:" + Ident);
			return new EntityIdent<K1,K2>(
				type, 
				(K1)Convert.ChangeType(keys[0], typeof(K1)), 
				(K2)Convert.ChangeType(keys[1], typeof(K2))
				);
		}
		public static EntityIdent<K1,K2,K3> Parse<K1,K2,K3>(string Ident)
			where K1 : IEquatable<K1>
			where K2 : IEquatable<K2>
			where K3 : IEquatable<K3>
		{
			var type = Parse(Ident, 0, Ident.Length, out var keys);
			if (keys.Length != 3)
				throw new ArgumentException("对象ID包含主键字段个数错误:" + Ident);
			return new EntityIdent<K1, K2,K3>(
				type,
				(K1)Convert.ChangeType(keys[0], typeof(K1)),
				(K2)Convert.ChangeType(keys[1], typeof(K2)),
				(K3)Convert.ChangeType(keys[2], typeof(K3))
				);
		}
		public static EntityIdent<K1, K2, K3, K4> Parse<K1,K2,K3,K4>(string Ident)
			where K1 : IEquatable<K1>
			where K2 : IEquatable<K2>
			where K3 : IEquatable<K3>
			where K4 : IEquatable<K4>
		{
			var type = Parse(Ident, 0, Ident.Length, out var keys);
			if (keys.Length != 4)
				throw new ArgumentException("对象ID包含主键字段个数错误:" + Ident);
			return new EntityIdent<K1, K2, K3, K4>(
				type,
				(K1)Convert.ChangeType(keys[0], typeof(K1)),
				(K2)Convert.ChangeType(keys[1], typeof(K2)),
				(K3)Convert.ChangeType(keys[2], typeof(K3)),
				(K4)Convert.ChangeType(keys[3], typeof(K4))
				);
		}
		public static string ToIdentString(this EntityIdent ident)
		{
			return Build(ident.Type, ident.Keys.Select(k => k.ToString()));
		}

	}

	public static class ServiceEntityIdent {
		public static string Create(long ServiceIdent, IEnumerable<string> Parts)
			=> EntityIdents.Build("SI", (Parts ?? Enumerable.Empty<string>()).WithFirst(ServiceIdent.ToString()));

		public static string Create(long ServiceIdent, params string[] Parts)
			=> EntityIdents.Build("SI", (IEnumerable<string>)Parts);

		public static string Create<K1>(long ServiceIdent, K1 Key1)
			=> EntityIdents.Build("SI", ServiceIdent.ToString(),Key1.ToString());

		public static string Create<K1,K2>(long ServiceIdent, K1 Key1, K2 Key2)
			=> EntityIdents.Build("SI", ServiceIdent.ToString(), Key1.ToString(), Key2.ToString());

		public static string Create<K1, K2, K3>(long ServiceIdent, K1 Key1, K2 Key2, K3 Key3)
			=> EntityIdents.Build("SI", ServiceIdent.ToString(), Key1.ToString(), Key2.ToString(), Key3.ToString());


		public static EntityIdent<long,K1> Parse<K1>(string Id)
			where K1:IEquatable<K1>
		{
			var re = EntityIdents.Parse<long, K1>(Id);
			if (re.Type != "SI")
				throw new ArgumentException($"指定标识不是服务实例的标识:{Id}");
			return re;
		}
		public static EntityIdent<long, K1,K2> Parse<K1,K2>(string Id)
			where K1 : IEquatable<K1>
			where K2 : IEquatable<K2>
		{
			var re = EntityIdents.Parse<long, K1,K2>(Id);
			if (re.Type != "SI")
				throw new ArgumentException($"指定标识不是服务实例的标识:{Id}");
			return re;
		}
		public static EntityIdent<long, K1,K2,K3> Parse<K1,K2,K3>(string Id)
			where K1 : IEquatable<K1>
			where K2 : IEquatable<K2>
			where K3 : IEquatable<K3>
		{
			var re = EntityIdents.Parse<long, K1,K2,K3>(Id);
			if (re.Type != "SI")
				throw new ArgumentException($"指定标识不是服务实例的标识:{Id}");
			return re;
		}
	}


}