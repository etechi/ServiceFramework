﻿#region Apache License Version 2.0
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

namespace SF.Sys.TimeServices
{
	public class TimeService : ITimeService
	{
		TimeSpan Offset;
		DateTime FloorToSeconds(DateTime time) =>
			new DateTime((time.Ticks / 10000000) * 10000000, time.Kind);

		public DateTime Now
		{
			get
			{
				var time = FloorToSeconds(DateTime.Now).Add(Offset);
				return time;
			}
		}

		public DateTime UtcNow
		{
			get
			{
				return FloorToSeconds(DateTime.UtcNow).Add(Offset);
			}
		}

		public void Reset()
		{
			Offset = TimeSpan.Zero;
		}

		public void SetTime(DateTime UtcNow)
		{
			Offset = UtcNow.Subtract(FloorToSeconds(DateTime.UtcNow));
		}
	}
}
