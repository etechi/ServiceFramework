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

using SF.Core.ServiceManagement;
using SF.Sys.Services;
using SF.Sys.Services.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Sys.Entities.AutoTest
{
	public interface IEntityTestDataProvider<TEditable>
	{
		Task<T> UseTestEntity<T>(int Count,Func<TEditable[], Task<T>> Action);
	}

	public interface IAutoTestEntity
	{
		Type EntityManagerType { get; }
	}
	public interface IAutoTestEntity<TManager> :IAutoTestEntity
	{ 
		Func<IServiceInstanceManager,IServiceInstanceInitializer<TManager>> ServiceConfig { get; }
	}
}
