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

namespace SF.Sys.NetworkService
{
	[NetworkService]
	public interface IServiceMetadataService
	{
		//[Authorize(Roles ="admin")]
		Metadata.Library Json();
		IContent Typescript(bool all = true);
		IContent TSD(string ApiName,string ResultFieldName=null, bool all = true);
		IContent Javascript(string ApiName,  bool all = true);
		IContent Java(
			string CommonImports,
			string PackagePath,
			bool all = true
			);
		IContent iOS(
			 string CommonImports,
			 string BaseService,
			 bool all = true
			 );
		IContent Html();
		IContent Script();
		IContent Angular();
	}
}
