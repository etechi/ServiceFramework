#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using System.Buffers;

namespace SF.Utils.Parsers
{
	public struct Content:IEquatable<Content>
	{
		Memory<char> Memory { get; }
		public bool IsEmpty => Memory.Length== 0;
		public int Length => Memory.Length;
		public override string ToString()
		{
			return new string(Memory.ToArray(), 0, Length);
		}
		
		public static Content From(string Str) =>
			new Content(Str.ToCharArray(), 0, Str.Length);
		public MemoryHandle Pin() => Memory.Pin();
		public static Content Empty { get; } = new Content(Array.Empty<char>(), 0, 0);
		public ReadOnlySpan<char> Span => Memory.Span;
		public char FirstChar => Length == 0 ? (char)0 : Memory.Span[0];
		public Content(char[] Buffer, int Offset, int Length)
		{
			this.Memory = new Memory<char>(Buffer, Offset, Length);
		}
		public Content(Memory<char> memory)
		{
			this.Memory = memory;
		}
		public Content Skip(int offset)
		{
			if (offset > Length)
				throw new ArgumentOutOfRangeException();
			return new Content(Memory.Slice(offset));
		}
		public Content Cut(int length)
		{
			if (length > Length)
				throw new ArgumentOutOfRangeException();
			return new Content(Memory.Slice(0, length));
		}

		public bool Equals(Content other)
		{
			return Span.SequenceEqual(other.Span);			
		}
		public override bool Equals(object obj)
		{
			if (obj is Content)
				return Equals((Content)obj);
			return false;
		}
		public override int GetHashCode()
		{
			return Memory.GetHashCode();
		}
	}
}

