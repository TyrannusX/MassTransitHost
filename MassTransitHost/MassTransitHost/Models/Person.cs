using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MassTransitHost.Models
{
    [XmlRoot]
    public class Person
    {
        public string FirstName { get; set; }
    }
}
