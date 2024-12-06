using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

public interface ISerializer
{
    void Serialize<T>(T obj, string path);
    T Deserialize<T>(string path);
}
public class BinarySerizalizer : ISerializer
{
    public void Serialize<T>(T obj, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(fs, obj);
        }
    }
    public T Deserialize<T>(string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            return (T)formatter.Deserialize(fs);
        }
    }
}
public class SoapSerizalizer : ISerializer
{
    public void Serialize<T>(T obj, string path)
    {
        SoapFormatter formatter = new SoapFormatter();

        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(fs, obj);
        }
    }
    public T Deserialize<T>(string path)
    {
        SoapFormatter formatter = new SoapFormatter();

        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            return (T)formatter.Deserialize(fs);
        }
    }
}
public class JsonSerializer : ISerializer
{
    public void Serialize<T>(T obj, string path)
    {
        string json = System.Text.Json.JsonSerializer.Serialize(obj);

        File.WriteAllText(path, json);
    }
    public T Deserialize<T>(string path)
    {
        string json = File.ReadAllText(path); 

        return System.Text.Json.JsonSerializer.Deserialize<T>(json);
    }
}
public class XmlSerializer : ISerializer
{
    public void Serialize<T>(T obj, string path)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(fs, obj);
        }
    }
    public T Deserialize<T>(string path)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            return (T)serializer.Deserialize(fs);
        }
    }
}
public enum CandyType
{
    Hard,
    Soft,
    Chewy,
    Gummy
}

[Serializable]
public struct CandyPackaging
{
    public string Material { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public CandyPackaging(string material, double width, double height)
    {
        Material = material;
        Width = width;
        Height = height;
    }
    public override string ToString()
    {
        return $"Material: {Material}, Width: {Width} cm, Height: {Height} cm";
    }
}
public interface ICloneable
{
    void DoClone();
}

[Serializable]
public abstract class BaseClone
{
    public abstract void DoClone();
}
public interface IEdible
{
    void Eat();
}

[Serializable]
public abstract class CandyProduct : BaseClone, ICloneable, IEdible
{
    public string Name { get; set; }
    public double Weight { get; set; }
    public CandyType Type { get; set; }
    public CandyPackaging Packaging { get; set; }
    public double SugarContent { get; set; }
    public CandyProduct(string name, double weight, CandyType type, CandyPackaging packaging, double sugarContent)
    {
        Name = name;
        Weight = weight;
        Type = type;
        Packaging = packaging;
        SugarContent = sugarContent;
    }
    public virtual void DisplayInfo()
    {
        Console.WriteLine($"Name: {Name}, Weight: {Weight}, Type: {Type}, Packaging: {Packaging}");
    }
    public override void DoClone()
    {
        Console.WriteLine("DoCloneMethod");
    }
    public abstract void Eat();
    public override string ToString()
    {
        return $"Type: {GetType()}, Name: {Name}, Weight: {Weight}, Candy Type: {Type}, Packaging: {Packaging}";
    }
    public override bool Equals(object obj)
    {
        return obj is CandyProduct other && Name == other.Name && Weight == other.Weight && Type == other.Type;
    }
    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 23 + Name.GetHashCode();
        hash = hash * 23 + Weight.GetHashCode();
        hash = hash * 23 + Type.GetHashCode();
        return hash;
    }
}

[Serializable]
public class Candy : CandyProduct
{
    public string Flavor { get; set; }
    public Candy() : base("", 0, CandyType.Hard, new CandyPackaging(), 0)
    {
        Flavor = string.Empty;
    }
    public Candy(string name, double weight, string flavor, CandyType type, CandyPackaging packaging, double sugarContent)
        : base(name, weight, type, packaging, sugarContent)
    {
        Flavor = flavor;
    }
    public override void DisplayInfo()
    {
        Console.WriteLine($"Name: {Name}, Weight: {Weight}, Flavor: {Flavor}, Type: {Type}, Packaging: {Packaging}");
    }
    public override void Eat()
    {
        Console.WriteLine($"You are eating {Flavor} flavored candy");
    }
}

namespace OOP13
{
    internal class Program
    {
        static void Main(string[] args)
        {

            // 1
            Candy candy = new Candy(
                "Lollipop", 
                50.0, 
                "Strawberry", 
                CandyType.Hard, 
                new CandyPackaging("Plastic", 5.0, 10.0), 
                sugarContent: 40.0);

            BinarySerizalizer binarySerializer = new BinarySerizalizer();
            SoapSerizalizer soapSerializer = new SoapSerizalizer();
            JsonSerializer jsonSerializer = new JsonSerializer();
            XmlSerializer xmlSerializer = new XmlSerializer();

            string binaryPath = "C:\\Users\\vlad\\source\\repos\\OOP13\\candy.dat";
            string soapPath = "C:\\Users\\vlad\\source\\repos\\OOP13\\candy_soap.xml";
            string jsonPath = "C:\\Users\\vlad\\source\\repos\\OOP13\\candy.json";
            string xmlPath = "C:\\Users\\vlad\\source\\repos\\OOP13\\candy.xml";

            Console.WriteLine("BINARY");
            binarySerializer.Serialize(candy, binaryPath);
            var binaryDeserialized = binarySerializer.Deserialize<Candy>(binaryPath);
            Console.WriteLine(binaryDeserialized);
            Console.WriteLine();

            Console.WriteLine("SOAP");
            soapSerializer.Serialize(candy, soapPath);
            var soapDeserialized = soapSerializer.Deserialize<Candy>(soapPath);
            Console.WriteLine(soapDeserialized);
            Console.WriteLine();

            Console.WriteLine("JSON");
            jsonSerializer.Serialize(candy, jsonPath);
            var jsonDeserialized = jsonSerializer.Deserialize<Candy>(jsonPath);
            Console.WriteLine(jsonDeserialized);
            Console.WriteLine();

            Console.WriteLine("XML");
            xmlSerializer.Serialize(candy, xmlPath);
            var xmlDeserialized = xmlSerializer.Deserialize<Candy>(xmlPath);
            Console.WriteLine(xmlDeserialized);
            Console.WriteLine();

            // 2
            var candies = new List<Candy>
            {
                new Candy("Gummy Bear", 30, "Apple", CandyType.Gummy, new CandyPackaging("Paper", 3.0, 5.0), 25.0),
                new Candy("Chocolate Bar", 100, "Chocolate", CandyType.Soft, new CandyPackaging("Foil", 10.0, 20.0), 60.0)
            };

            Console.WriteLine("XML list:");
            xmlSerializer.Serialize(candies, xmlPath);
            var deserializedXML = xmlSerializer.Deserialize<List<Candy>>(xmlPath);
            foreach (var candyEl in deserializedXML)
            {
                Console.WriteLine(candyEl);
            }
            Console.WriteLine();

            // 3
            XmlDocument document = new XmlDocument();

            document.Load("C:\\Users\\vlad\\source\\repos\\OOP13\\candy.xml");

            XmlNodeList names = document.SelectNodes("/ArrayOfCandy/Candy/Name");
            Console.WriteLine("Candy names:");
            foreach (XmlNode name in names)
            {
                Console.WriteLine(name.InnerText);
            }

            XmlNodeList candiesWithSugar = document.SelectNodes("/ArrayOfCandy/Candy[SugarContent > 30]");
            Console.WriteLine("Candies with SugarContent > 30:");
            foreach (XmlNode candyWithSugarContent in candiesWithSugar)
            {
                Console.WriteLine(candyWithSugarContent.SelectSingleNode("Name").InnerText);
            }
            Console.WriteLine();

            // 4
            XDocument doc = XDocument.Load("C:\\Users\\vlad\\source\\repos\\OOP13\\candy.xml");

            Console.WriteLine("Candy names:");
            var candyNames = doc.Descendants("Candy").Select(c => c.Element("Name").Value);
            foreach (var name in candyNames)
            {
                Console.WriteLine(name);
            }
            Console.WriteLine();

            Console.WriteLine("Candy packaging:");
            var packagingNames = doc.Descendants("Packaging").Select(p => p.Element("Material").Value);
            foreach (var material in packagingNames)
            {
                Console.WriteLine(material);
            }
        }
    }
}
