using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ids4CenterApp
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public ResourceOwnerPasswordValidator() { }

        public Claim[] GetUserClaims()
        {
            return new Claim[]
            {
                new Claim("UserId",1.ToString()),
                new Claim(JwtClaimTypes.Name,"ljh"),
                new Claim(JwtClaimTypes.GivenName,"liujiaheng"),
                new Claim(JwtClaimTypes.FamilyName,"liu"),
                new Claim(JwtClaimTypes.Email,"431440303@qq.com"),
                new Claim(JwtClaimTypes.Role,"admin")
            };
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (context.UserName == "ljh" && context.Password == "haha")
            {
                context.Result = new GrantValidationResult(
                    subject: context.UserName,
                    authenticationMethod: "custom",
                    claims: GetUserClaims()
                    );

            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom grant");
            }

            return Task.CompletedTask;
        }
    }
}
