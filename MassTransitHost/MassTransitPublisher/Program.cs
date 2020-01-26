using MassTransit;
using MassTransitHost.Models;
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
            var bus = Bus.Factory.CreateUsingRabbitMq();
            var endpoint = await bus.GetSendEndpoint(new Uri("rabbitmq://localhost/test_queue")).ConfigureAwait(false);

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
            await endpoint.Send(myMessage).ConfigureAwait(false);
        }
    }
}
