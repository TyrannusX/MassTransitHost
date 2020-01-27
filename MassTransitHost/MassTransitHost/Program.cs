using MassTransitHost.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace MassTransitHost
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return (int)HostFactory.Run(cfg =>
            {
                cfg.Service<EventConsumerService>(x =>
                {
                    x.ConstructUsing(y => new EventConsumerService());
                    x.WhenStarted((y, hostControl) => y.Start(hostControl));
                    x.WhenStopped((y, hostControl) => y.Stop(hostControl));
                    x.WhenShutdown((y, hostControl) => y.Stop(hostControl));
                });
                cfg.RunAsLocalSystem();
                cfg.SetDescription("My Subscriber Server");
                cfg.SetDisplayName("MT-Sub-Service");
                cfg.SetServiceName("MT-Sub-Service");
                cfg.StartAutomatically();
            });
        }
    }
}
