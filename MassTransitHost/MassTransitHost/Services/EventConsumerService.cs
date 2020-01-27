using GreenPipes;
using MassTransit;
using MassTransit.Serialization;
using MassTransitHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Topshelf;
using MassTransit.QuartzIntegration;

namespace MassTransitHost.Services
{
    public class EventConsumerService : ServiceControl
    {
        private IBusControl _busControl;
        private MessageProcessor _processor = new MessageProcessor();

        public bool Start(HostControl hostControl)
        {
            _busControl = ConfigureBus();
            _busControl.Start();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _busControl.Stop();
            return true;
        }

        private IBusControl ConfigureBus()
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("amqp://guest:guest@localhost/local_vh");
                cfg.ReceiveEndpoint("test_queue", e =>
                {
                    e.UseConcurrencyLimit(5);
                    e.UseMessageRetry(r =>
                    {
                        r.Exponential(10, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
                    });
                    e.Consumer<MessageProcessor>();
                });
            });
        }
    }
}
