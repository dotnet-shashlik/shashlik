#nullable enable
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Shashlik.Utils.Extensions
{
    public static class XmlExtensions
    {
        /// <summary>
        /// serialize to xml string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="omitXmlDeclaration">omit xml declaration, e.g. : &lt;?xml version="1.0" encoding="utf-8"?&gt;</param>
        /// <param name="encoding">encode, default utf8</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ToXmlString<T>(this T value, bool omitXmlDeclaration = false, Encoding? encoding = null)
        {
            if (value is null)
                return string.Empty;

            encoding ??= Encoding.UTF8;
            var xmlSerializer = new XmlSerializer(typeof(T));
            using var ms = new MemoryStream();
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = omitXmlDeclaration,
                Encoding = encoding
            };
            using var writer = XmlWriter.Create(ms, settings);
            xmlSerializer.Serialize(writer, value);

            return ms.ReadToString(encoding);
        }

        /// <summary>
        /// serialize to xml string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="settings">custom settings</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ToXmlString<T>(this T value, XmlWriterSettings settings)
        {
            if (value is null)
                return string.Empty;

            var xmlSerializer = new XmlSerializer(typeof(T));
            using var ms = new MemoryStream();
            using var writer = XmlWriter.Create(ms, settings);
            xmlSerializer.Serialize(writer, value);
            return ms.ReadToString();
        }

        /// <summary>
        /// serialize to xml string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="settings">custom settings</param>
        /// <param name="namespaces">namespaces</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ToXmlString<T>(this T value, XmlWriterSettings settings, XmlSerializerNamespaces namespaces)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (namespaces == null) throw new ArgumentNullException(nameof(namespaces));
            if (value is null)
                return string.Empty;

            var xmlSerializer = new XmlSerializer(typeof(T));
            using var ms = new MemoryStream();
            using var writer = XmlWriter.Create(ms, settings);
            xmlSerializer.Serialize(writer, value, namespaces);
            return ms.ReadToString();
        }


        /// <summary>
        /// deserialize string to type: <typeparamref name="T"/> 
        /// </summary>
        /// <param name="xml"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static T DeserializeXml<T>(this string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(xml));
            var xmlSerializer = new XmlSerializer(typeof(T));
            var stringReader = new StringReader(xml);
            using var reader = XmlReader.Create(stringReader, new XmlReaderSettings
            {
                CheckCharacters = false
            });
            return (T) xmlSerializer.Deserialize(reader);
        }

        /// <summary>
        /// deserialize string to type: <typeparamref name="T"/> 
        /// </summary>
        /// <param name="xml">xml string</param>
        /// <param name="settings">custom settings</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static T DeserializeXml<T>(this string xml, XmlReaderSettings settings)
        {
            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(xml));
            var xmlSerializer = new XmlSerializer(typeof(T));
            var stringReader = new StringReader(xml);
            using var reader = XmlReader.Create(stringReader, settings);
            return (T) xmlSerializer.Deserialize(reader);
        }
    }
}