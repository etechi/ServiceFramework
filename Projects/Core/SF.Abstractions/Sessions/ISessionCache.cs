using ServiceProtocol;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Security.Sessions
{

    public interface ISessionCache<T>
    {
        Task Remove(string key);
        Task<T> Get(string key, Func<string, T> Deserializer);
        Task<string> Create(T ticket, DateTime expires, Func<T, string> Serializer);
        Task Update(string key, T ticket, DateTime expires, Func<T, string> Serializer);
    }

}
