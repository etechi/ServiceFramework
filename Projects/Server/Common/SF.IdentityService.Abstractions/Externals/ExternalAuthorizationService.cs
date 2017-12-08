using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.Auth.Storages;
using Microsoft.AspNet.Identity;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Security.Claims;
using System.Net.Http;
using ServiceProtocol.Annotations;
namespace ServiceProtocol.Auth.Externals
{
    [Annotations.RequireServiceType(typeof(IExternalAuthorizationProvider),"外部认证")]
    public abstract class ExternalAuthorizationService<
        TKey, 
        TExtAuthDescriptor,
        TExtAuthDescriptorManager, 
        TUserIdentService
        > :
		IExternalAuthorizationService
		where TExtAuthDescriptorManager:IExtAuthDescriptorManager<TExtAuthDescriptor>
		where TExtAuthDescriptor:ExtAuthDescriptor
		where TUserIdentService:IUserIdentService<TKey>
		where TKey:IEquatable<TKey>
	{
        public DI.IDIProviderResolver<IExternalAuthorizationProvider> Resolver { get; }
		public TUserIdentService UserIdentService { get; }
        public TExtAuthDescriptorManager ExtAuthDescriptorManager { get; }
        public Caching.ICache Cache { get; }
        static readonly string CacheKeyPrefix = "SP.AUTH.EXT.STATE:";
        public ExternalAuthorizationService(
			DI.IDIProviderResolver<IExternalAuthorizationProvider> Resolver,
			TUserIdentService UserIdentService,
			TExtAuthDescriptorManager ExtAuthDescriptorManager,
            Caching.ICache Cache
            )
		{
            this.Cache = Cache;
			this.Resolver = Resolver;
			this.UserIdentService = UserIdentService;
			this.ExtAuthDescriptorManager = ExtAuthDescriptorManager;
		}
		class State
        {
            public string Source { get; set; }
            public string Invitor { get; set; }
            public string Value { get; set; }
        }

        public async Task<string> GetAuthState(string Source, string Invitor, string State)
        {
            var stateKey = Guid.NewGuid().ToString("N");
            await Cache.SetAsync(CacheKeyPrefix + stateKey, new State
            {
                Invitor = Invitor,
                Source = Source,
                Value = State
            }, DateTime.Now.AddHours(1));
            return stateKey;

        }
        public async Task<HttpResponseMessage> StartAuthorization(
            string Provider, 
            string ClientType,
            string Callback,
            string Source,
            string Invitor,
            string State
            )
		{
			var provider = Resolver.Resolve(Provider);
            var stateKey = await GetAuthState(Source, Invitor, State);
            return await provider.StartAuthorization(
                stateKey, 
                ClientType, 
                Callback
                );
		}
		protected abstract Task<TKey> OnSignup(
            byte IdentType,
            UserInfo User,
            string Source,
            string Invitor,
            AccessLocation AccessLocation
            );
		protected abstract Task<HttpResponseMessage> OnSignin(HttpRequestMessage Request, TKey UserId,string State);

		public async Task<HttpResponseMessage> ProcessCallback(
            string Provider, 
            HttpRequestMessage Request,
            AccessLocation AccessLocation
            )
		{
			var provider = Resolver.Resolve(Provider);
			var re = await provider.ProcessCallback(Request);
            if(re==null)
                return await OnSignin(Request, default(TKey), null);
            var providerDescriptor = await ExtAuthDescriptorManager.FindByIdAsync(Provider);
            var state = (State)Cache.Get(CacheKeyPrefix + re.State);
            if(state==null)
                return await OnSignin(Request, default(TKey), null);

            var iid = await UserIdentService.FindUserIdByExtIdent(providerDescriptor.Id, re.ExtIdent);
			if(iid.Equals(default(TKey)))
			{
				var ui=await provider.GetUserInfo(re.Token);
                iid = await UserIdentService.FindUserIdByIdent(providerDescriptor.IdentType, ui.Ident);
                if (iid.Equals(default(TKey)))
                {
                   
                    iid = await OnSignup(providerDescriptor.IdentType, ui, state.Source, state.Invitor, AccessLocation);
                }
                await UserIdentService.BindUserExtIdent(providerDescriptor.Id, ui.ExtIdent, providerDescriptor.IdentType, ui.Ident, iid);
			}
            return await OnSignin(Request, iid, state.Value);
		}

	}
}
