using Application.Common.Settings;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class EdgeGapConfigurationProvider
{
    public EdgeGapConfiguration? Data { get; set; }

    public EdgeGapConfigurationProvider(IConfiguration configuration)
    {
        Data = new EdgeGapConfiguration()
        {
            AppName = configuration.GetValue<string>("EdgeGapConfiguration:AppName"),
            AppVersion = configuration.GetValue<string>("EdgeGapConfiguration:AppVersion"),
            DeployUrl = configuration.GetValue<string>("EdgeGapConfiguration:DeployUrl"),
            WebHookUrl = configuration.GetValue<string>("EdgeGapConfiguration:WebHookUrl")
        };
    }
}
