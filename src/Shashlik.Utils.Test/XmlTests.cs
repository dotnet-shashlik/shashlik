using System;
using Shashlik.Utils.Extensions;
using Shouldly;
using Xunit;

namespace Shashlik.Utils.Test
{
    public class XmlTests
    {
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.chinatax.gov.cn/dataspec/", IsNullable = false, ElementName = "taxML")]
        public class InvoiceTokenResponse
        {
            public string token { get; set; }
        }

        [Fact]
        public void Tests()
        {
            var xml = @"<taxML xsi:type="""" xmlbh="""" bbh=""1.0"" xmlmc="""" xsi:schemaLocation=""http://www.chinatax.gov.cn/dataspec"" xmlns=""http://www.chinatax.gov.cn/dataspec/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><token>e6777bd11c2443389729c46de71ca001</token></taxML>";
            var obj = xml.DeserializeXml<InvoiceTokenResponse>();
            obj.token.ShouldBe("e6777bd11c2443389729c46de71ca001");
            InvoiceTokenResponse nullObj = null;
            nullObj.ToXmlString().ShouldBeEmpty();
            var newObj = new InvoiceTokenResponse(){token = "test"};
            newObj.ToXmlString().DeserializeXml<InvoiceTokenResponse>().token.ShouldBe(newObj.token);
            newObj.ToXmlString(false, false).DeserializeXml<InvoiceTokenResponse>().token.ShouldBe(newObj.token);

            Should.Throw<Exception>(() => "".DeserializeXml<object>());
        }
    }
}