using MassTransit;
using MassTransitHost.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml;
using System.Xml.Serialization;

namespace MassTransitHost.Services
{
    public class MessageProcessor : IConsumer<MyMessage>
    {
        public async Task Consume(ConsumeContext<MyMessage> context)
        {
            Console.WriteLine($"Received message {context.Message.Xml} with retry count {context.GetRetryCount()}");
            var xmlSerializer = new XmlSerializer(typeof(Person));
            var streamReader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(context.Message.Xml)));
            streamReader.Namespaces = false;
            var person = (Person)xmlSerializer.Deserialize(streamReader);
            Console.WriteLine($"First name is {person.FirstName}");
            throw new Exception();
        }
    }
}
