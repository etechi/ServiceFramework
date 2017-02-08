using System;
using System.Collections.Generic;

namespace SF.Data.Entity
{
	public abstract class EntityIdent
	{
		public abstract int KeyCount { get; }
		public abstract Type[] KeyTypes { get; }
		public abstract object[] Keys { get; }
	}
	public class EntityIdent<K1> : EntityIdent
		where K1 : IEquatable<K1>
	{
		public K1 Id1 { get; set; }
		public override int KeyCount => 1;
		public override Type[] KeyTypes => new[] { typeof(K1) };
		public override object[] Keys => new[] { (object)Id1 };
	}
	public class EntityIdent<K1, K2> : EntityIdent
		where K1:IEquatable<K1>
		where K2:IEquatable<K2>
	{
		public K1 Id1 { get; set; } 
		public K2 Id2 { get; set; }
		public override int KeyCount => 2;
		public override Type[] KeyTypes => new[] { typeof(K1), typeof(K2) };
		public override object[] Keys => new[] { (object)Id1,(object)Id2 };
	}
	public class EntityIdent<K1, K2,K3> : EntityIdent
		where K1 : IEquatable<K1>
		where K2 : IEquatable<K2>
		where K3 : IEquatable<K3>
	{
		public K1 Id1 { get; set; }
		public K2 Id2 { get; set; }
		public K3 Id3 { get; set; }
		public override int KeyCount => 3;
		public override Type[] KeyTypes => new[] { typeof(K1), typeof(K2), typeof(K3) };
		public override object[] Keys => new[] { (object)Id1, (object)Id2, (object)Id3 };
	}
	public class EntityIdent<K1, K2, K3,K4> : EntityIdent
	   where K1 : IEquatable<K1>
	   where K2 : IEquatable<K2>
	   where K3 : IEquatable<K3>
	   where K4 : IEquatable<K4>
	{
		public K1 Id1 { get; set; }
		public K2 Id2 { get; set; }
		public K3 Id3 { get; set; }
		public K4 Id4 { get; set; }
		public override int KeyCount => 4;
		public override Type[] KeyTypes => new[] { typeof(K1), typeof(K2), typeof(K3), typeof(K4) };
		public override object[] Keys => new[] { (object)Id1, (object)Id2, (object)Id3, (object)Id4 };
	}

	public static class EntityIdents
	{
		public static string Build(string Type, params object[] Idents)
		{
			return Type + "-" + string.Join("-", Idents);
		}
		public static string Build<T>(string Type, T Ident)
		{
			return Type + "-" + Convert.ToString(Ident);
		}
		public static KeyValuePair<string, string[]> Parse(string Ident)
		{
			var i = Ident.IndexOf('-');
			if (i == -1)
				throw new ArgumentException();
			var type = Ident.Substring(0, i);
			var keys = Ident.Substring(i + 1).Split('-');
			if (string.IsNullOrWhiteSpace(type) || keys.Length == 0)
				throw new ArgumentException("对象ID格式错误：" + Ident);
			return new KeyValuePair<string, string[]>(type, keys);
		}
		public static KeyValuePair<string, T> ParseSingleKey<T>(string Ident)
		{
			var re = Parse(Ident);
			if (re.Value.Length != 1)
				throw new ArgumentException("对象ID包含多个主键字段:" + Ident);
			var id = (T)Convert.ChangeType(re.Value[0], typeof(T));
			return new KeyValuePair<string, T>(re.Key, id);
		}
		public static T ParseSingleKey<T>(string Ident, string Type)
		{
			var p = ParseSingleKey<T>(Ident);
			if (p.Key != Type)
				throw new ArgumentException($"数据对象类型错误：希望类型：{Type} 实际类型:{p.Key}");
			return p.Value;
		}
		public static string[] Parse(string Ident, string Type)
		{
			var p = Parse(Ident);
			if (p.Key != Type)
				throw new ArgumentException($"数据对象类型错误：希望类型：{Type} 实际类型:{p.Key}");
			return p.Value;
		}
	}
}
