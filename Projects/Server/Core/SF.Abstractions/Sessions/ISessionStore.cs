using ServiceProtocol;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Security.Sessions
{
    public class SessionData
    {
        public string Id { get; set; }
        public string Session { get; set; }
        public DateTime Expires { get; set; }
    }
    public interface ISessionStore
    {
        Task Remove(string key);
        Task<SessionData> Get(string key);
        Task Update(SessionData data);
    }
}
