using System;
using System.Linq;
using System.IO;
using System.Text;
using Shouldly;
using Xunit;

namespace Saturn72.FileConverters.Tests
{
    public class ExcelToJsonFileConverterTests
    {
        #region consts

        private const string SomeDataExpectedJson =
            "[{\"A\":\"1_1\",\"B\":\"1_2\",\"C\":\"1_3\",\"D\":\"1_4\"},{\"A\":\"2_1\",\"B\":\"2_2\",\"C\":\"\",\"D\":\"2_4\"},{\"A\":\"3_1\",\"B\":\"3_2\",\"C\":\"3_3\",\"D\":\"3_4\"},{\"A\":\"\",\"B\":\"4_2\",\"C\":\"4_3\",\"D\":\"4_4\"},{\"A\":\"5_1\",\"B\":\"\",\"C\":\"5_3\",\"D\":\"5_4\"},{\"A\":\"6_1\",\"B\":\"6_2\",\"C\":\"6_3\",\"D\":\"6_4\"},{\"A\":\"7_1\",\"B\":\"7_2\",\"C\":\"\",\"D\":\"7_4\"}]";
        #endregion
        #region SuppotedConversions
        [Fact]
        public void ExcelToJsonFileConverter_SupportedConversions()
        {
            var e2jc = new ExcelToJsonFileConverter();
            var sc = e2jc.SupportedConversions;
            sc.Count().ShouldBe(2);

            sc.Any(c => c.SourceExtension == "xls" && c.DestinationExtension == "json").ShouldBeTrue();
            sc.Any(c => c.SourceExtension == "xlsx" && c.DestinationExtension == "json").ShouldBeTrue();
        }
        #endregion

        #region Convert

        [Fact]
        public void ExcelToJsonFileConverter_Convert_ThrowsOnUnsupportted()
        {

            using (var ms = new MemoryStream(new byte[] { 1, 1, 0, 0 }))
            {
                Should.Throw<NotSupportedException>(() => new ExcelToJsonFileConverter().Convert("dadada", "json", ms));

                Should.Throw<NotSupportedException>(() => new ExcelToJsonFileConverter().Convert("xls", "ss", ms));

                Should.Throw<NotSupportedException>(() => new ExcelToJsonFileConverter().Convert("xlsx", "ss", ms));

                Should.Throw<NotSupportedException>(() => new ExcelToJsonFileConverter().Convert("s", "ss", ms));
            }
        }

        [Theory]
        [InlineData("empty.xls")]
        [InlineData("empty.xlsx")]
        public void ExcelToJsonFileConverter_Converts_EmptyFile(string fileName)
        {
            var path = Path.Combine("Resources", fileName);
            var extension = Path.GetExtension(fileName).Replace(".", "");
            var bytes = File.ReadAllBytes(path);
            var e2jc = new ExcelToJsonFileConverter();
            using (var ms = new MemoryStream(bytes))
            {
                e2jc.Convert(extension, "json", ms).Count().ShouldBe(0);
            }
        }

        [Theory]
        [InlineData("some-data.xls")]
        [InlineData("some-data.xlsx")]
        public void ExcelToJsonFileConverter_Converts(string fileName)
        {
            
            var path = Path.Combine("Resources", fileName);
            var extension = Path.GetExtension(fileName).Replace(".", "");
            var bytes = File.ReadAllBytes(path);
            var e2jc = new ExcelToJsonFileConverter();
            using (var ms = new MemoryStream(bytes))
            {
                var converted = e2jc.Convert(extension, "json", ms);
                var actualJson = Encoding.UTF8.GetString(converted);
                actualJson.ShouldBe(SomeDataExpectedJson);
            }
        }

        #endregion

        #region ToJsonString

        [Theory]
        [InlineData(null, "")]
        [InlineData("simple-string", "simple-string")]
        [InlineData("simple'string", "simple\\'string")]
        [InlineData("simple\"string", "simple\\\"string")]
        public void ExcelToJsonFileConverter_ToJsonString(string source, string expected)
        {
            ExcelToJsonFileConverterTestObject.GetJsonString(source).ShouldBe(expected);
        }
        #endregion

        #region Test Objects

        public class ExcelToJsonFileConverterTestObject : ExcelToJsonFileConverter
        {
            public static string GetJsonString(String source)
            {
                return ToJsonString(source);
            }
        }
        #endregion
    }
}
