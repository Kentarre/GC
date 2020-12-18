using GC.Api.Interfaces;
using GC.Backend;
using GC.Backend.Interfaces;
using GC.Backend.States;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GC.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            .ConfigureServices((c,s) =>
            {
                s.AddSingleton<IStateProvider, StateProvider>();
                s.AddTransient<IDeliverMessages, MessagesDelivery>();
                s.AddTransient<IGenerateNicknames, NicknameGenerator>();
            });
    }
}
