using System;
using System.Collections.Generic;
using System.Linq;

namespace SF.Entities
{
	public abstract class EntityIdent
	{
		public abstract string Type { get; }
		public abstract int KeyCount { get; }
		public abstract Type[] KeyTypes { get; }
		public abstract object[] Keys { get; }
		public override string ToString()
		{
			return Type + "-" + string.Join("-", Keys);
		}
	}
	public class CommonEntityIdent : EntityIdent
	{
		public override string Type { get; }
		public override int KeyCount => Keys.Length;
		Lazy<Type[]> Types { get; }
		public override Type[] KeyTypes => Types.Value;
		public override object[] Keys { get; }
		public CommonEntityIdent(string Type,string[] Keys)
		{
			this.Type = Type;
			Types = new Lazy<Type[]>(() => Enumerable.Repeat(typeof(string), Keys.Length).ToArray());
			this.Keys = Keys;
		}
	}
	public class EntityIdent<K1> : EntityIdent
		where K1 : IEquatable<K1>
	{
		public K1 Id1 { get; }
		public override string Type { get; }

		public EntityIdent(string Type,K1 Id1)
		{
			this.Type = Type;
			this.Id1 = Id1;
		}
		public override int KeyCount => 1;
		public override Type[] KeyTypes => new[] { typeof(K1) };
		public override object[] Keys => new[] { (object)Id1 };
	}
	public class EntityIdent<K1, K2> : EntityIdent
		where K1:IEquatable<K1>
		where K2:IEquatable<K2>
	{
		public K1 Id1 { get; } 
		public K2 Id2 { get; }
		public override string Type { get; }

		public EntityIdent(string Type, K1 Id1,K2 Id2)
		{
			this.Type = Type;
			this.Id1 = Id1;
			this.Id2 = Id2;
		}
		public override int KeyCount => 2;
		public override Type[] KeyTypes => new[] { typeof(K1), typeof(K2) };
		public override object[] Keys => new[] { (object)Id1,(object)Id2 };
	}
	public class EntityIdent<K1, K2,K3> : EntityIdent
		where K1 : IEquatable<K1>
		where K2 : IEquatable<K2>
		where K3 : IEquatable<K3>
	{
		public K1 Id1 { get;  }
		public K2 Id2 { get; }
		public K3 Id3 { get; }
		public override string Type { get; }

		public EntityIdent(string Type, K1 Id1, K2 Id2,K3 Id3)
		{
			this.Type = Type;
			this.Id1 = Id1;
			this.Id2 = Id2;
			this.Id3 = Id3;
		}
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
		public K1 Id1 { get;  }
		public K2 Id2 { get; }
		public K3 Id3 { get; }
		public K4 Id4 { get; }
		public override string Type { get; }

		public EntityIdent(string Type, K1 Id1, K2 Id2, K3 Id3,K4 Id4)
		{
			this.Type = Type;
			this.Id1 = Id1;
			this.Id2 = Id2;
			this.Id3 = Id3;
			this.Id4 = Id4;
		}
		public override int KeyCount => 4;
		public override Type[] KeyTypes => new[] { typeof(K1), typeof(K2), typeof(K3), typeof(K4) };
		public override object[] Keys => new[] { (object)Id1, (object)Id2, (object)Id3, (object)Id4 };
	}

	
}
