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
            "[{\"String\":\"One\",\"Boolean\":1,\"Byte\":\"0000\",\"D\":\"1_4\"},{\"String\":\"Two\",\"Boolean\":0,\"Byte\":\"0001\",\"D\":\"2_4\"},{\"String\":\"Three\",\"Boolean\":1,\"Byte\":\"0010\",\"D\":\"3_4\"},{\"String\":\"\",\"Boolean\":\"\",\"Byte\":\"\",\"D\":\"4_4\"},{\"String\":\"Five\",\"Boolean\":0,\"Byte\":\"0011\",\"D\":\"5_4\"}]";
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
        [InlineData("some-data.csv")]
        public void CsvToJsonFileConverter_Converts(string fileName)
        {

            var path = Path.Combine(ResourceFolder, fileName);
            var extension = Path.GetExtension(fileName).Replace(".", "");
            var bytes = File.ReadAllBytes(path);
            var c2jc = new CsvToJsonFileConverter();
            var converted = c2jc.Convert(extension, "json", bytes, null);
            var actualJson = Encoding.UTF8.GetString(converted);
            actualJson.ShouldBe(SomeDataExpectedJson);
        }

        #endregion
    }
}
