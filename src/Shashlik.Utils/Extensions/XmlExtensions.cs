using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            try
            {
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
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }
    }
}
