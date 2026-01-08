using Application.Common.Interfaces;
using Application.Common.Settings;
using Application.Models.EdgeGap;
using Infrastructure.Common.Models;
using Infrastructure.Common.Settings;
using Infrastructure.Consumers;
using Infrastructure.HttpClients.EdgeGap;
using Infrastructure.Persistence;
using Infrastructure.Publishers;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.SignalRHubs.MessageBus;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace Infrastructure;

/// <summary>
/// Extension Class For <see cref="IServiceCollection"/> Interface
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    /// Injects Infrastructure Dependencies Into Dependency Injection Container
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/> Interface</param>
    /// <param name="configuration"><see cref="IConfiguration"/> Interface</param>
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddTransient<IDeploymentPublisher, DeploymentPublisher>();
        services.AddTransient<IServerPublisher, ServerPublisher>();
        services.AddTransient<IServerRepository, ServerRepository>();
        services.AddTransient<ITokenGenerationService, TokenGenerationService>();
        services.AddScoped<IServerHttpClient<EdgeGapDeploymentResult>, EdgeGapHttpClient>();
        services.AddTransient<IServerNotifierService, ServerNotifierService>();

#if DEBUG
        var edgeGapSettings = new EdgeGapSettings();
        configuration.GetSection("EdgeGapSettings").Bind(edgeGapSettings);
        edgeGapSettings.BaseUrl = configuration.GetValue<string>("EdgeGapSettings:BaseUrl");
        edgeGapSettings.Token = configuration.GetValue<string>("EdgeGapSettings:Token");
        
#else
        var edgeGapStringSettings = Environment.GetEnvironmentVariable("EdgeGapSettings");
        var edgeGapSettings = JsonConvert.DeserializeObject<EdgeGapSettings>(edgeGapStringSettings);

        var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(edgeGapStringSettings,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        EdgeGapConfiguration cfgFromJson = new();
        if (!string.IsNullOrWhiteSpace(edgeGapStringSettings))
        {
            if (dict is not null)
            {

                cfgFromJson = new EdgeGapConfiguration
                {
                    AppName = dict.GetValueOrDefault("AppName"),
                    AppVersion = dict.GetValueOrDefault("AppVersion"),
                    DeployUrl = dict.GetValueOrDefault("DeployUrl"),
                    DeleteUrl = dict.GetValueOrDefault("DeleteUrl"),
                    WebHookUrl = dict.GetValueOrDefault("WebHookUrl"),
                    CreateVersionUrl = dict.GetValueOrDefault("CreateVersionUrl"),
                    DeleteVersionUrl = dict.GetValueOrDefault("DeleteVersionUrl"),
                    RegistryUserName = dict.GetValueOrDefault("RegistryUserName"),
                    RegistryPassword = dict.GetValueOrDefault("RegistryPassword"),
                };
            }
        }

        services.AddOptions<EdgeGapConfiguration>()
            .Configure<IConfiguration>((opt, conf) =>
            {
                // base from JSON
                opt.AppName = cfgFromJson.AppName;
                opt.AppVersion = cfgFromJson.AppVersion;
                opt.DeployUrl = cfgFromJson.DeployUrl;
                opt.DeleteUrl = cfgFromJson.DeleteUrl;
                opt.WebHookUrl = cfgFromJson.WebHookUrl;
                opt.CreateVersionUrl = cfgFromJson.CreateVersionUrl;
                opt.DeleteVersionUrl = cfgFromJson.DeleteVersionUrl;
                opt.RegistryUserName = cfgFromJson.RegistryUserName;
                opt.RegistryPassword = cfgFromJson.RegistryPassword;

                // overrides from normal configuration, если есть
                conf.GetSection("EdgeGapConfiguration").Bind(opt);
            });
#endif

        services.AddHttpClient<IEdgeGapHttpClient, EdgeGapHttpClient>(client =>
        {
            client.BaseAddress = new Uri(uriString: edgeGapSettings.BaseUrl);
            client.DefaultRequestHeaders.Add("authorization", edgeGapSettings.Token);
        });

        services.AddMassTransit(busConfig =>
        {
            busConfig.SetKebabCaseEndpointNameFormatter(); //user-created-event
            busConfig.AddConsumer<MatchNewConsumer>();
            //busConfig.AddConsumer<MatchStatusUpdateConsumer>();
            busConfig.AddConsumer<MatchCancelConsumer>();
            busConfig.AddConsumer<MatchStartConsumer>();
            busConfig.AddConsumer<MatchEndConsumer>();

#if DEBUG
            var settings = new MessageBrokerSettings();
            configuration.GetSection("MessageBroker").Bind(settings);

            Console.WriteLine(settings.Username);
#else
            var stringSettings = Environment.GetEnvironmentVariable("MessageBroker");
            var settings = JsonConvert.DeserializeObject<MessageBrokerSettings>(stringSettings);
#endif
            busConfig.UsingRabbitMq((context, configuration) =>
            {

                configuration.Host(new Uri(settings.Host!), h =>
                {
                    h.Username(settings.Username!);
                    h.Password(settings.Password!);
                });

                configuration.ReceiveEndpoint("match-cancel-server", e =>
                {
                    e.ConfigureConsumer<MatchCancelConsumer>(context);
                });

                configuration.UseInMemoryOutbox(context);

                configuration.ConfigureEndpoints(context);
            });
        });


#if DEBUG
        var redisSettings = new RedisSettings();
        configuration.GetSection("RedisSettings").Bind(redisSettings);
        Console.WriteLine("Redis Host: " + redisSettings.Host);
#else
        var redisStringSettings = Environment.GetEnvironmentVariable("RedisSettings");
        var redisSettings = JsonConvert.DeserializeObject<RedisSettings>(redisStringSettings);
#endif

        var config = new ConfigurationOptions
        {
            EndPoints = { redisSettings.Host },
            User = redisSettings.User,
            Password = redisSettings.Password,
            AbortOnConnectFail = false
        };

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(config));

        services.AddSignalR(option =>
        {
            option.KeepAliveInterval = TimeSpan.FromSeconds(5);
            option.ClientTimeoutInterval = TimeSpan.FromSeconds(15);
        })
        .AddStackExchangeRedis(o =>
        {
            o.ConnectionFactory = async writer =>
            {
                var config = new ConfigurationOptions
                {
                    EndPoints = { redisSettings.Host },
                    User = redisSettings.User,
                    Password = redisSettings.Password,
                    AbortOnConnectFail = false
                };
                config.SetDefaultPorts();
                var connection = await ConnectionMultiplexer.ConnectAsync(config, writer);
                connection.ConnectionFailed += (_, e) =>
                {
                    Console.WriteLine("Connection to Redis failed.");
                };

                if (!connection.IsConnected)
                {
                    Console.WriteLine("Did not connect to Redis.");
                }

                return connection;
            };
        });


        if (Convert.ToBoolean(configuration.GetValue<bool>("UseInMemoryDatabase")))
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("TestDb"));
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                string connString = string.Empty;
#if DEBUG
                connString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
#else
                connString = Environment.GetEnvironmentVariable("CONNSTRING");
#endif
                options.UseNpgsql(connString,
                    builder =>
                    {
                        builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        //EF allows you to specify that a given LINQ query should be split into multiple SQL queries.
                        //Instead of JOINs, split queries generate an additional SQL query for each included collection navigation
                        //More about that: https://docs.microsoft.com/en-us/ef/core/querying/single-split-queries
                        builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    });
            });
        }

        services.AddScoped<IApplicationDbContext>(x => x.GetService<ApplicationDbContext>()!);
    }
}