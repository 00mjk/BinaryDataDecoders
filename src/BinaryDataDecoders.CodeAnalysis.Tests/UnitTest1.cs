using BinaryDataDecoders.CodeAnalysis.CSharp;
using BinaryDataDecoders.TestUtilities;
using BinaryDataDecoders.ToolKit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace BinaryDataDecoders.CodeAnalysis.Tests
{
    [TestClass]
    public class UnitTest1
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestMethod1()
        {
            XPathNavigator nav = new CSharpAnalyzer()
                   .Analyze(@"C:\Repos\mwwhited\BinaryDataDecoders\src\BinaryDataDecoders.Apple2\Dos33\AppleFileType.cs");

            var node = nav.SelectSingleNode("hello/world");
        }

        //  [TestMethod]
        [DataTestMethod]
        [DataRow(@"C:\Repos\mwwhited\BinaryDataDecoders\src\BinaryDataDecoders.CodeAnalysis\Xml\XPath\XPathNodeTypeEx.cs")]
        [DataRow(@"C:\Repos\mwwhited\BinaryDataDecoders\src\BinaryDataDecoders.Apple2\ApplesoftBASIC\Detokenizer.cs")]
        [DataRow(@"C:\Repos\mwwhited\BinaryDataDecoders\src\BinaryDataDecoders.Apple2\Dos33\CatalogEntry.cs")]
        [DataRow(@"C:\Repos\mwwhited\BinaryDataDecoders\src\BinaryDataDecoders.Apple2\Dos33\AppleFileType.cs")]
        public void DeeperTest(string cs)
        {
            var xsltArgumentList = new XsltArgumentList();

            var templateName = "SimpleCopy.xslt";
            using var templateStream = this.GetResourceStream(templateName);

            var xslt = new XslCompiledTransform(false);
            using var xmlreader = XmlReader.Create(templateStream, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                ConformanceLevel = ConformanceLevel.Document,
                NameTable = new NameTable(),
            });
            var xsltSettings = new XsltSettings(false, false);
            xslt.Load(xmlreader, xsltSettings, null);

            using var resultStream = new MemoryStream();

            //var cs = @"C:\Repos\mwwhited\BinaryDataDecoders\src\BinaryDataDecoders.CodeAnalysis\Xml\XPath\XPathNodeTypeEx.cs";
            // var cs = @"C:\Repos\mwwhited\BinaryDataDecoders\src\BinaryDataDecoders.Apple2\ApplesoftBASIC\Detokenizer.cs";
            //var cs = @"C:\Repos\mwwhited\BinaryDataDecoders\src\BinaryDataDecoders.Apple2\Dos33\CatalogEntry.cs";
            // var cs = @"C:\Repos\mwwhited\BinaryDataDecoders\src\BinaryDataDecoders.Apple2\Dos33\AppleFileType.cs";
            XPathNavigator nav = new CSharpAnalyzer().Analyze(cs);

            xslt.Transform(nav, xsltArgumentList, resultStream);

            this.TestContext.AddResult(resultStream);

            resultStream.Position = 0;
            var xml = XDocument.Load(resultStream);

            this.TestContext.AddResult(xml);
        }

        [TestMethod]
        public void JustXPathTest()
        {
            XPathNavigator nav = new CSharpAnalyzer()
                   .Analyze(@"C:\Repos\mwwhited\BinaryDataDecoders\src\BinaryDataDecoders.Apple2\Dos33\AppleFileType.cs");

            var x = nav.Clone();
            var xml = nav.OuterXml;

            this.TestContext.AddResult(xml);

        }

        [TestMethod]
        public void BuildTreeTest()
        {
            var cs = @"C:\Repos\mwwhited\BinaryDataDecoders\src\BinaryDataDecoders.Apple2\Dos33\AppleFileType.cs";
            var nav = new CSharpAnalyzer().Pointer(cs);

            static XElement toXml(ISyntaxPointer sp) => new XElement(sp.Name, sp.Children.Select(toXml));

            var xml = toXml(nav);
        }
    }
}