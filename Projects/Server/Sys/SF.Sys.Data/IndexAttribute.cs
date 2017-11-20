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
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Data
{
	[AttributeUsage(AttributeTargets.Property,AllowMultiple =true)]
	public class IndexAttribute : Attribute
	{
		public IndexAttribute() { }
		public IndexAttribute(string name) { this.Name = name; }
		public IndexAttribute(string name, int order)
		{
			this.Name = name;
			this.Order = order;
		}
		public virtual bool IsClustered { get; set; }
		public virtual bool IsUnique { get; set; }
		public virtual string Name { get; set; }
		public virtual int Order { get; set; }
	}

}
