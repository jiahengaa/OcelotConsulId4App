using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static Ids4CenterApp.Models.RequestTokenParam;

namespace Ids4CenterApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration configuration;
        public LoginController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        // POST api/Login/doLogin
        [Route("doLogin")]
        [HttpPost]
        public async Task<ActionResult> doLogin([FromBody]LoginRequestParam model)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["client_id"] = model.ClientId;
            dict["client_secret"] = configuration[$"IdentityClients:{model.ClientId}:ClientSecret"];
            dict["grant_type"] = configuration[$"IdentityClients:{model.ClientId}:GrantType"];
            dict["username"] = model.UserName;
            dict["password"] = model.Password;

            using (HttpClient http = new HttpClient())
            using (var content = new FormUrlEncodedContent(dict))
            {
                var msg = await http.PostAsync(configuration["IdentityService:TokenUri"], content);
                if (!msg.IsSuccessStatusCode)
                {
                    return StatusCode(Convert.ToInt32(msg.StatusCode));
                }

                string result = await msg.Content.ReadAsStringAsync();
                return Content(result, "application/json");
            }
        }

        [Route("getsome")]
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "aaa", "vv" };
        }
    }
}
