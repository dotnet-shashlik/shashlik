using System;
using System.Text;
using Shashlik.Utils.Extensions;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.Utils.Test
{
    public class XmlTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public XmlTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.chinatax.gov.cn/dataspec/",
            IsNullable = false, ElementName = "taxML")]
        public class InvoiceTokenResponse
        {
            public string token { get; set; }
        }

        [Fact]
        public void Tests()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var xml =
                @"<taxML xsi:type="""" xmlbh="""" bbh=""1.0"" xmlmc="""" xsi:schemaLocation=""http://www.chinatax.gov.cn/dataspec"" xmlns=""http://www.chinatax.gov.cn/dataspec/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><token>e6777bd11c2443389729c46de71ca001乱七八糟的文字</token></taxML>";
            var obj = xml.DeserializeXml<InvoiceTokenResponse>();
            obj.token.ShouldBe("e6777bd11c2443389729c46de71ca001乱七八糟的文字");

            {
                InvoiceTokenResponse nullObj = null;
                nullObj.ToXmlString().ShouldBeEmpty();
                var newObj = new InvoiceTokenResponse() {token = "test"};
                newObj.ToXmlString().DeserializeXml<InvoiceTokenResponse>().token.ShouldBe(newObj.token);
                newObj.ToXmlString(true).DeserializeXml<InvoiceTokenResponse>().token
                    .ShouldBe(newObj.token);
            }

            {
                var xml1 = obj.ToXmlString(false, Encoding.GetEncoding("GBK"));
                var xml2 = obj.ToXmlString(true, Encoding.GetEncoding("GBK"));

                _testOutputHelper.WriteLine(xml1);
                _testOutputHelper.WriteLine(xml2);

                xml1.DeserializeXml<InvoiceTokenResponse>().token.ShouldBe(obj.token);
                xml2.DeserializeXml<InvoiceTokenResponse>().token.ShouldBe(obj.token);
            }

            Should.Throw<Exception>(() => "".DeserializeXml<object>());
        }
    }
}