using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Adapter
{

    public class Product
    {
        public Product() { }

        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public static class ProductDataProvider
    {
        public static List<Product> GetData() =>
        new List<Product>
        {
            new Product("Iphone", 5000),
            new Product("Samsung S8", 4000),
            new Product("Samsung S9", 7000)
        };
    }

    public class XmlConverter
    {
        public XDocument GetXml()
        {
            var xDocument = new XDocument();
            var xElement = new XElement("Productos");
            var xAttributes = ProductDataProvider.GetData().Select(m => new XElement("Producto",
                new XAttribute("Nombre", m.Name), new XAttribute("Precio", m.Price)));
            xElement.Add(xAttributes);
            xDocument.Add(xElement);
            return xDocument;
        }
    }

    public interface IXmlToJson
    {
        void ConvertXmlToJson();
    }

    public class XmlToJsonAdapter : IXmlToJson
    {
        private XmlConverter _xmlConverter;

        public XmlToJsonAdapter(XmlConverter xmlConverter)
        {
            _xmlConverter = xmlConverter;
        }

        public void ConvertXmlToJson()
        {
            var products = _xmlConverter.GetXml()
                .Element("Productos")
                .Elements("Producto")
                .Select(m=> new Product
                {
                    Name= m.Attribute("Nombre").Value,
                    Price= decimal.Parse( m.Attribute("Precio").Value)
                });
            // new JsonConverter(products).GetJson();
            new JsonConverter(products).ConvertToJson();
        }
    }

    public class JsonConverter
    {
        private IEnumerable<Product> _productData;

        public JsonConverter(IEnumerable<Product> productData)
        {
            _productData = productData;
        }

        public void ConvertToJson()
        {
            var result = JsonConvert.SerializeObject( _productData, Formatting.Indented);
            Console.WriteLine(result);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ADAPTER" + '\n');
            Console.WriteLine("Trata de comunicar interfaces que son distintas para que puedan funcionar entre si. Por ejemplo para adaptar informacion de distintos formatos entre 2 Apis" + '\n');

            Console.WriteLine(new XmlConverter().GetXml());

            Console.WriteLine();
          
            var xmlConverter = new XmlConverter();
            var adapter = new XmlToJsonAdapter(xmlConverter);
            adapter.ConvertXmlToJson();
            Console.ReadLine();
        }
    }
}
