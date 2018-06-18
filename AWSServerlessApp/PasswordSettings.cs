using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AWSServerlessApp
{
    public static class PasswordSettings
    {
        public static IConfiguration Configuration { get; set; }

        public static T Get<T>(string key)
        {
            if (Configuration == null)
            {
                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                var configuration = builder.Build();
                Configuration = configuration.GetSection("PasswordSettings");
            }

            return (T)Convert.ChangeType(Configuration[key], typeof(T));
        }
    }
}
