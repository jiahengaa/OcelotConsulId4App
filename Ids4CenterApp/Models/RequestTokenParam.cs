using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ids4CenterApp.Models
{
    public class RequestTokenParam
    {
        public class LoginRequestParam
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string ClientId { get; set; }
        }
    }
}
