using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using UnitIoC;
using Unity;


IConfiguration config = new ConfigurationBuilder()
                                    .AddJsonFile("appSettings.json")
                                    .AddEnvironmentVariables()
                                    .Build();
using IHost host = Host.CreateDefaultBuilder(args).
                   ConfigureServices((_, services) =>
                   {
                       services.AddHangfire(x => x.UseSqlServerStorage(config.GetSection("Local").Value));
                       services.AddHangfireServer();
                   })
                   .Build();

GlobalConfiguration.Configuration.UseSqlServerStorage(config.GetSection("Local").Value);

var container = new UnityContainer();

container.RegisterType<ICar, BMW>();

//var driver = container.Resolve<Driver>();
//driver.RunCar();

RecurringJob.AddOrUpdate<ICar>("getConsole", m => m.Run(), Cron.Minutely);

using (var server = new BackgroundJobServer())
{
    Console.WriteLine("Hangfire Server started. Press any key to exit...");
    Console.ReadKey();
}