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
using System.Collections.Generic;
using System.Linq;

namespace SF.Sys.Entities
{
	public class TrackIdent
    {
        public string Type { get; }
        public string IdentType { get; }
        public long Ident { get; }
        public string Extension { get; }
        public TrackIdent(string Type,string IdentType,long Ident,string Extension=null)
        {
            this.Type = Type;
            this.IdentType = IdentType;
            this.Ident = Ident;
            this.Extension = Extension;
        }
        public override string ToString()
        {
            var re=$"{Type}:{IdentType}-{Ident}";
            if (Extension.HasContent())
                re += ":" + Extension;
            return re;
        }
        public static bool TryParse(string s,out TrackIdent Ident)
        {
            Ident = null;
            var i = s.IndexOf(':');
            var j = s.IndexOf('-', i + 1);
            if (i == -1 || j == -1)
                return false;
            var k = s.IndexOf(':', j + 1);
            string Extension = null;
            if (k != -1)
                Extension = s.Substring(k + 1);
            else
                k = s.Length;
            var bizType = s.Substring(0, i);
            var bizIdentType = s.Substring(i + 1, j - i - 1);
            if (!long.TryParse(s.Substring(j + 1,k-j-1), out var bizIdent))
                return false;
            Ident = new TrackIdent(bizType, bizIdentType, bizIdent, Extension);
            return true;
        }
        public static TrackIdent Parse(string s)
        {
            if (!TryParse(s, out var ti))
                throw new ArgumentException("无效的跟踪标识:" + s);
            return ti;
        }
        public static implicit operator string(TrackIdent id)
        {
            return id.ToString();
        }
        public static implicit operator TrackIdent(string id)
        {
            return Parse(id);
        }
    }	
}
