using System;
using System.Collections.Generic;
using System.Text;

namespace AdminByRequestChallange.Contracts
{
    public class SessionResponse
    {
        public string SessionKey { get; set; }
        public long Expiration { get; set; }
    }
}
