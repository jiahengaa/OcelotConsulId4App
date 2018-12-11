﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace APIGatewayByOcelot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // 因为在Ocelot.json文件中配置了访问ApiOne应用中的接口必须要经过授权，
            // 所以客户端要想访问ApiOne应用中的接口，就必须带上AccessToken，然后Ocelot拿着这个AccessToken去Ids4Center验证合法性，
            // 如果验证通过就把请求转发到ApiOne应用处理
            var authenticationProviderApiOneServerKey = "ApiOneServerKey";
            var authenticationProviderApiTwoServerKey = "ApiTwoServerKey";

            Action<IdentityServerAuthenticationOptions> apiOneServerOption = option =>
            {
                option.Authority = "http://localhost:12345";
                option.ApiName = "api-one";
                option.RequireHttpsMetadata = false;
                option.SupportedTokens = SupportedTokens.Both;
                option.ApiSecret = "api-one-secret";
            };

            Action<IdentityServerAuthenticationOptions> apiTwoServerOption = option =>
            {
                option.Authority = "http://localhost:12345";
                option.ApiName = "api-two";
                option.RequireHttpsMetadata = false;
                option.SupportedTokens = SupportedTokens.Both;
                option.ApiSecret = "api-two-secret";
            };

            services.AddAuthentication()
                    .AddIdentityServerAuthentication(authenticationProviderApiOneServerKey, apiOneServerOption)
                    .AddIdentityServerAuthentication(authenticationProviderApiTwoServerKey, apiTwoServerOption);

            services.AddOcelot();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            //app.UseMvc();
            app.UseOcelot().Wait();
        }
    }
}
