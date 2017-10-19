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

using SF.Auth;
using SF.Core.ServiceManagement.Models;
using SF.Entities;
using SF.Metadata;
using System;
namespace SF.Core.ServiceManagement.Management
{
	public class ServiceImplementQueryArgument : IQueryArgument<ObjectKey<string>>
	{
		[Comment("ID")]
		public ObjectKey<string> Id { get; set; }

		[Comment("����ʵ������")]
		public string Name { get; set; }

		[Comment("����ʵ�ַ���")]
		public string Group { get; set; }

		[EntityIdent(typeof(ServiceDeclaration))]
		[Comment("������")]
		public string DeclarationId { get; set; }
	}

	[EntityManager]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("����ʵ�ֹ���", "ϵͳ���÷���ʵ��")]
	[Category("ϵͳ����", "ϵͳ�������")]
	public interface IServiceImplementManager :
		IEntitySource<ObjectKey<string>, Models.ServiceImplement, ServiceImplementQueryArgument>
	{

	}
}
