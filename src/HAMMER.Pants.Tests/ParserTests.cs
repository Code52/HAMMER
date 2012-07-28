using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Assert = Xunit.Assert;

namespace HAMMER.Pants.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void Parse_WithWhiteText_UpdatesSpecificKeys()
        {
            // arrange
            var document = XDocument.Load("SimpleResourceFile.xaml");
            var parser = new ResourceFileParser(document);
            
            // act
            var output = parser.Update("FFFFFF");

            // assert
            var themeResources = output.Descendants()
                .Where(c => c.Name.LocalName == "SolidColorBrush");

            if (!themeResources.Any())
                throw new AssertFailedException("No theme resources found");

            foreach (var x in themeResources)
            {
                var theme = x.Parent.Attribute(ResourceFileParser.XamlRootNamespace + "Key");
                if (theme.Value == "HighContrast")
                    continue;

                Assert.Equal(x.Attribute("Color").Value, "#FFFFFFFF");
            }
        }
    }
}
