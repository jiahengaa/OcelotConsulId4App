using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ids4CenterApp
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
            Config.Configuration = this.Configuration;
            services.AddIdentityServer()
               .AddDeveloperSigningCredential()//扩展程序为签名令牌创建临时密钥材料
               .AddInMemoryIdentityResources(Config.GetIdentityResources())
               .AddInMemoryApiResources(Config.GetApiResources())//IResourceStore根据ApiResource配置对象的内存中集合注册实现
               .AddInMemoryClients(Config.GetClients())//基于配置对象的内存中集合的注册IClientStore和ICorsPolicyService实现Client
               .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
               .AddProfileService<ProfileService>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseIdentityServer();//注入HTTP管道中
            app.UseMvc();
        }
    }
}
