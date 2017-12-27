using System;
using System.Linq;
using System.IO;
using System.Text;
using Shouldly;
using Xunit;

namespace Saturn72.FileConverters.Tests
{
    public class CsvToJsonFileConverterTests
    {
        #region consts

        private const string ResourceFolder = "CsvResources";

        private const string SomeDataExpectedJson =
            "[{\"SKU\":1, \"Name\":\"Name_1\", \"Description\":\"description with \\\"qualifier - 1\\\' \\\"\", \"Price\":1.1, \"Available\":true},{\"SKU\":2, \"Name\":\"Name_2\", \"Description\":\"description with \\\"qualifier - 2\\\' \\\"\", \"Price\":2.2, \"Available\":true},{\"SKU\":3, \"Name\":\"Name_3\", \"Description\":\"description with \\\"qualifier - 3\\\' \\\"\", \"Price\":3.3, \"Available\":true},{\"SKU\":4, \"Name\":\"Name_4\", \"Description\":\"description with \\\"qualifier - 4\\\' \\\"\", \"Price\":4.4, \"Available\":true},{\"SKU\":5, \"Name\":\"Name_5\", \"Description\":\"description with \\\"qualifier - 5\\\' \\\"\", \"Price\":5.5, \"Available\":true},{\"SKU\":6, \"Name\":\"Name_6\", \"Description\":\"description with \\\"qualifier - 6\\\' \\\"\", \"Price\":6.6, \"Available\":true}]";
        #endregion

        #region SuppotedConversions
        [Fact]
        public void CsvToJsonFileConverter_SupportedConversions()
        {
            var c2jc = new CsvToJsonFileConverter();
            var sc = c2jc.SupportedConversions;
            sc.Count().ShouldBe(1);

            sc.Any(c => c.SourceExtension == "csv" && c.DestinationExtension == "json").ShouldBeTrue();
        }
        #endregion

        #region Convert

        [Fact]
        public void CsvToJsonFileConverter_Convert_Throws()
        {
            var c2jc = new CsvToJsonFileConverter();

            //nullreference 
            Should.Throw<NullReferenceException>(() => c2jc.Convert("csv", "json", null, null));
            //unsupported
            var bytes = new byte[] { 1, 1, 0, 0 };
            Should.Throw<NotSupportedException>(() => c2jc.Convert("dadada", "json", bytes, null));

            Should.Throw<NotSupportedException>(() => c2jc.Convert("csv", "ss", bytes, null));

            Should.Throw<NotSupportedException>(() => c2jc.Convert("s", "ss", bytes, null));
        }

        [Theory]
        [InlineData("empty.csv")]
        public void CsvToJsonFileConverter_Converts_EmptyFile(string fileName)
        {
            var path = Path.Combine(ResourceFolder, fileName);
            var extension = Path.GetExtension(fileName).Replace(".", "");
            var bytes = File.ReadAllBytes(path);
            var c2jc = new CsvToJsonFileConverter();
            c2jc.Convert(extension, "json", bytes, null).Count().ShouldBe(0);
        }

        [Theory]
        [InlineData("good-with-qualifier-tab.csv")]
        [InlineData("good-with-qualifier-comma.csv")]
        public void CsvToJsonFileConverter_Converts(string fileName)
        {

            var path = Path.Combine(ResourceFolder, fileName);
            var extension = Path.GetExtension(fileName).Replace(".", "");
            var bytes = File.ReadAllBytes(path);
            var c2jc = new CsvToJsonFileConverter();
            var delimiter = fileName.IndexOf("tab", StringComparison.InvariantCultureIgnoreCase) > 0 ? '\t' : ',';
            var converted = c2jc.Convert(extension, "json", bytes, new {@delimiter = delimiter});
            var actualJson = Encoding.UTF8.GetString(converted);
            actualJson.ShouldBe(SomeDataExpectedJson);
        }

        #endregion
    }
}
