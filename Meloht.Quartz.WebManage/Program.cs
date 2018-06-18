using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Meloht.Quartz.WebManage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "计划任务调度程序";
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            return WebHost.CreateDefaultBuilder(args)
                        .UseUrls(config.GetValue<string>("StartHost:RootHost"))
                        .UseStartup<Startup>()
                        .Build();
        }
    }
}
