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

using SF.Auth.Users.Internals;
using SF.Auth.Users.Models;
using SF.Core.ServiceManagement;
using System.Threading.Tasks;

namespace SF.Auth.Users.IdentityCredentialProviders
{
	public abstract class BaseIdentityCredentialProvider :
		IUserCredentialProvider
	{
		protected IUserCredentialStorage CredentialStorage { get; }
		public IServiceInstanceDescriptor ServiceInstance { get; }
		public abstract string Name { get; }
		public long Id => ServiceInstance.InstanceId;

		public BaseIdentityCredentialProvider(
			IUserCredentialStorage CredentialStorage,
			IServiceInstanceDescriptor ServiceInstance
			)
		{
			this.CredentialStorage = CredentialStorage;
			this.ServiceInstance = ServiceInstance;
		}

		public virtual Task Bind(string Ident, string UnionIdent, bool Confirmed, long UserId)
			=> CredentialStorage.Bind(ServiceInstance.InstanceId, Ident, UnionIdent, Confirmed, UserId);


		public virtual Task<UserCredential> Find(string Ident, string UnionIdent)
			=> CredentialStorage.Find(ServiceInstance.InstanceId, Ident, UnionIdent);

		public virtual Task<UserCredential> FindOrBind(string Ident, string UnionIdent, bool Confirmed, long UserId)
			=> CredentialStorage.FindOrBind(ServiceInstance.InstanceId, Ident, UnionIdent, Confirmed, UserId);

		public virtual Task<UserCredential[]> GetIdents(long UserId)
			=> CredentialStorage.GetIdents(ServiceInstance.InstanceId, UserId);

		public virtual Task SetConfirmed(string Ident, bool Confirmed)
			=> CredentialStorage.SetConfirmed(ServiceInstance.InstanceId, Ident, Confirmed);

		public virtual Task Unbind(string Ident, long UserId)
			=> CredentialStorage.Unbind(ServiceInstance.InstanceId, Ident, UserId);

		public abstract Task<string> VerifyFormat(string Ident);
		public abstract bool IsConfirmable();
		public abstract Task<long> SendConfirmCode(long? IdentityId, string Ident, string Code, ConfirmMessageType Type, string TrackIdent);
	}
}
