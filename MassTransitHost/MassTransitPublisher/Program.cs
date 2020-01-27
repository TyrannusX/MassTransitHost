using MassTransit;
using MassTransit.Context;
using MassTransitHost.Models;
using RabbitMQ.Client.Framing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MassTransitPublisher
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //configure bus
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("amqp://guest:guest@localhost/local_vh");
                cfg.ConfigureSend(s => s.UseSendExecute(c => c.Headers.Set("CorrelationId", Guid.NewGuid().ToString())));
            });
            var endpoint = await bus.GetSendEndpoint(new Uri("amqp://guest:guest@localhost/local_vh/test_queue")).ConfigureAwait(false);

            //create and serialize object
            var person = new Person() { FirstName = "LolzCat" };
            var xmlSerializer = new XmlSerializer(typeof(Person));
            var xml = string.Empty;

            using (var stringWriter = new Utf8StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter))
                {
                    var ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    xmlSerializer.Serialize(xmlWriter, person, ns);
                    xml = stringWriter.ToString();
                }
            }

            //send
            var myMessage = new MyMessage() { Xml = xml };
            for (int i = 0; i < 1; i++)
                await endpoint.Send(myMessage).ConfigureAwait(false);
        }
    }
}
