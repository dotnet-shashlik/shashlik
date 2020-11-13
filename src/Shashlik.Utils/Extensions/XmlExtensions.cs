using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Shashlik.Utils.Extensions
{
    public static class XmlExtensions
    {
        public static string ToXmlString<T>(this T value, bool noNamespace = true, bool noHeader = true)
        {
            if (value == null)
            {
                return string.Empty;
            }
            var xmlSerializer = new XmlSerializer(typeof(T));
            var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = noHeader
            };
            using var writer = XmlWriter.Create(stringWriter, settings);
            if (noNamespace)
            {
                xmlSerializer.Serialize(writer, value, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));
            }
            else
            {
                xmlSerializer.Serialize(writer, value);
            }
            return stringWriter.ToString();
        }
        
        public static T DeserializeXml<T>(this string xml)
        {
            if (string.IsNullOrEmpty(xml)) throw new NotSupportedException("Empty string!!");

            var xmlSerializer = new XmlSerializer(typeof(T));
            var stringReader = new StringReader(xml);
            using var reader = XmlReader.Create(stringReader);
            return (T)xmlSerializer.Deserialize(reader);
        }
    }
}
