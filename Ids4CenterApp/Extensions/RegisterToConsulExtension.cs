using System;
using System.Linq;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ids4CenterApp.Extensions
{
    public static class RegisterToConsulExtension
    {
        /// <summary>
        /// 添加Consul功能
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            string ip = configuration["consulip"];
            string port = configuration["consulport"];

            Console.WriteLine("####consulip : " + ip);
            Console.WriteLine("####consulport:" + port);

            // 配置Consul服务注册地址
            services.Configure<ServiceDiscoveryOptions>(configuration.GetSection("ServiceDiscovery"));
            // 配置Consul客户端
            services.AddSingleton<IConsulClient>(sp => new ConsulClient(config =>
            {
                var consulOptions = sp.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;
                if (!string.IsNullOrWhiteSpace(consulOptions.Consul.HttpEndPoint))
                {
                    config.Address = new Uri(consulOptions.Consul.HttpEndPoint);
                }

                if (!string.IsNullOrWhiteSpace(ip) && !string.IsNullOrWhiteSpace(port))
                {
                    config.Address = new Uri($"http://{ip}:{port}");
                }
            }));

            return services;
        }

        /// <summary>
        /// 使用Consul
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, string servername = "")
        {
            IConsulClient consul = app.ApplicationServices.GetRequiredService<IConsulClient>();
            IApplicationLifetime appLife = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            IOptions<ServiceDiscoveryOptions> serviceOptions = app.ApplicationServices.GetRequiredService<IOptions<ServiceDiscoveryOptions>>();
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));

            // 向Consul客户端注册RestApi服务
            foreach (var address in addresses)
            {
                Console.WriteLine("####server name : " + ((!string.IsNullOrWhiteSpace(servername)) ? servername : serviceOptions.Value.ServiceName));
                Console.WriteLine("####address:" + address.Host + ":" + address.Port);

                var serviceId = $"{((!string.IsNullOrWhiteSpace(servername)) ? servername : serviceOptions.Value.ServiceName)}_{address.Host}:{address.Port}";

                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                    Interval = TimeSpan.FromSeconds(30),
                    HTTP = new Uri(address, "HealthCheck").OriginalString
                };

                var registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    Address = address.Host,
                    ID = serviceId,
                    Name = ((!string.IsNullOrWhiteSpace(servername)) ? servername : serviceOptions.Value.ServiceName),
                    Port = address.Port
                };

                consul.Agent.ServiceRegister(registration).GetAwaiter().GetResult();

                // 服务应用停止后发注册RestApi服务
                appLife.ApplicationStopping.Register(() =>
                {
                    consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
                });
            }

            return app;
        }
    }
}