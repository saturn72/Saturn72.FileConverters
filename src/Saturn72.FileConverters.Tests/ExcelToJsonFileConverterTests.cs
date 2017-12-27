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

        private const string ResourceFolder = "ExcelResources";
        private const string XlsExtension = "xls";
        private const string XlsxExtension = "xlsx";
        private const string JsonExtension = "json";

        private const string SomeDataExpectedJson =
            "[{\"String\":\"One\",\"Boolean\":1,\"Byte\":\"0000\",\"D\":\"1_4\"},{\"String\":\"Two\",\"Boolean\":0,\"Byte\":\"0001\",\"D\":\"2_4\"},{\"String\":\"Three\",\"Boolean\":1,\"Byte\":\"0010\",\"D\":\"3_4\"},{\"String\":\"\",\"Boolean\":\"\",\"Byte\":\"\",\"D\":\"4_4\"},{\"String\":\"Five\",\"Boolean\":0,\"Byte\":\"0011\",\"D\":\"5_4\"}]";
        #endregion

        #region SuppotedConversions
        [Fact]
        public void ExcelToJsonFileConverter_SupportedConversions()
        {
            var e2jc = new ExcelToJsonFileConverter();
            var sc = e2jc.SupportedConversions;
            sc.Count().ShouldBe(2);

            sc.Any(c => c.SourceExtension == XlsExtension && c.DestinationExtension == JsonExtension).ShouldBeTrue();
            sc.Any(c => c.SourceExtension == XlsxExtension && c.DestinationExtension == JsonExtension).ShouldBeTrue();
        }
        #endregion

        #region Convert

        [Theory]
        [InlineData(XlsExtension)]
        [InlineData(XlsxExtension)]
        public void ExcelToJsonFileConverter_Convert_ThrowsnOnNullByteArray(string srcExt)
        {
            var e2jc = new ExcelToJsonFileConverter();
            Should.Throw<NullReferenceException>(() => e2jc.Convert(srcExt, JsonExtension, null, null));
        }

        [Theory]
        [InlineData("dadada", JsonExtension)]
        [InlineData(XlsExtension, "ss")]
        [InlineData(XlsxExtension, "ss")]
        [InlineData("s", "ss")]
        public void ExcelToJsonFileConverter_Convert_ThrowsnUnsupportted(string srcEx, string destExt)
        {
           //not supported
            var e2jc = new ExcelToJsonFileConverter();
            Should.Throw<NotSupportedException>(() => e2jc.Convert(srcEx, destExt, new byte[]{1,0,0,1}, null));
        }

        [Theory]
        [InlineData("empty.xls")]
        [InlineData("empty.xlsx")]
        public void ExcelToJsonFileConverter_Converts_EmptyFile(string fileName)
        {
            var path = Path.Combine(ResourceFolder, fileName);
            var extension = Path.GetExtension(fileName).Replace(".", "");
            var bytes = File.ReadAllBytes(path);
            var e2jc = new ExcelToJsonFileConverter();
            e2jc.Convert(extension, JsonExtension, bytes, null).Count().ShouldBe(0);
        }

        [Theory]
        [InlineData("some-data.xls")]
        [InlineData("some-data.xlsx")]
        public void ExcelToJsonFileConverter_Converts(string fileName)
        {

            var path = Path.Combine(ResourceFolder, fileName);
            var extension = Path.GetExtension(fileName).Replace(".", "");
            var bytes = File.ReadAllBytes(path);
            var e2jc = new ExcelToJsonFileConverter();
            var converted = e2jc.Convert(extension, JsonExtension, bytes, null);
            var actualJson = Encoding.UTF8.GetString(converted);
            actualJson.ShouldBe(SomeDataExpectedJson);
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
            public static string GetJsonString(string source)
            {
                return ToJsonConvertFunc(typeof(string))(source) as string;
            }
        }
        #endregion
    }
}
