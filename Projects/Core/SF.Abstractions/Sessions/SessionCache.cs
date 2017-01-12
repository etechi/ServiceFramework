using ServiceProtocol;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Security.Sessions
{
    public class SessionCache<T> : ISessionCache<T>
    {
        ServiceProtocol.Caching.ICache Cache { get; }
        ISessionStore Store { get; }
        static string SessionPrefex = "/SP/SESS/";
        static string SessionKey = "啊的￥实打#；爱神%…实的…&*（就箭";
        
        public SessionCache(Caching.ICache Cache, ISessionStore Store)
        {
            this.Cache = Cache;
            this.Store = Store;
        }

        string BuildSession(string id)
        {
            return id.Sha1Sign(SessionKey+ExtSystemTag.Value);
        }
        string ParseSession(string sess)
        {
            return sess.Sha1Unsign(SessionKey + ExtSystemTag.Value);
        }
        public async Task<string> Create(T ticket, DateTime expires, Func<T, string> Serializer)
        {
            var id = Guid.NewGuid().ToString("N");
            await Store.Update(new SessionData
            {
                Id = id,
                Expires = expires,
                Session = Serializer(ticket)
            });
            await Cache.SetAsync(SessionPrefex+id, ticket, expires);
            return BuildSession(id);
        }

        public async Task<T> Get(string key, Func<string, T> Deserializer)
        {
            var id = ParseSession(key);
            if (string.IsNullOrEmpty(id))
                return default(T);
            var re = await Cache.GetAsync(SessionPrefex + id);
            if (re != null)
                return (T)re;
            var data = await Store.Get(id);
            if (data == null)
                return default(T);
            var ticket = Deserializer(data.Session);
            await Cache.SetAsync(SessionPrefex + id, ticket, data.Expires);
            return ticket;
        }

        public async Task Remove(string key)
        {
            var id = ParseSession(key);
            if (string.IsNullOrEmpty(id))
                return;

            await Store.Remove(id);
            await Cache.RemoveAsync(SessionPrefex + id);
        }

        public async Task Update(string key, T ticket, DateTime expires, Func<T, string> Serializer)
        {
            var id = ParseSession(key);
            if (string.IsNullOrEmpty(id))
                return;
            await Store.Update(
                new SessionData {
                    Id = id,
                    Expires = expires,
                    Session = Serializer(ticket)
                });
            await Cache.SetAsync(SessionPrefex + id, ticket, expires);
        }
    }

}
