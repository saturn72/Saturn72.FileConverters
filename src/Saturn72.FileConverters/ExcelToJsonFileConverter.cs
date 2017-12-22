using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using fastJSON;

namespace Saturn72.FileConverters
{
    public class ExcelToJsonFileConverter : IFileConverter
    {
        #region ctor

        static ExcelToJsonFileConverter()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        #endregion

        private static readonly IEnumerable<FileConversionData> SupporttedConversions = new[]
        {
            new FileConversionData(XlsExtension, JsonExtension),
            new FileConversionData(XlsxExtension, JsonExtension)
        };

        public IEnumerable<FileConversionData> SupportedConversions => SupporttedConversions;

        public byte[] Convert(string sourceExtension, string destinationExtension, Stream stream)
        {
            if (!this.IsSupported(sourceExtension, destinationExtension))
                throw new NotSupportedException(
                    string.Format(
                        "The required conversion is not supported (From {0} file extension sto to {1} file extension)",
                        sourceExtension, destinationExtension));

            using (var excelReader = CreateExcelDataReader(sourceExtension, stream))
            {
                if(!excelReader.Read())
                    return new byte[]{};

                var columNamesAndIndexes = GetColumnNamesAndIndexes(excelReader);
                var jsonArray = new List<IDictionary<string, string>>();

                while (excelReader.Read())
                {
                    var jsonArrayItem = new Dictionary<string, string>();
                    foreach (var cnni in columNamesAndIndexes)
                    {
                        var key = ToJsonString(cnni.Key);
                        var value = ToJsonString(excelReader.GetString(cnni.Value));
                        jsonArrayItem[key] = value;

                    }
                    jsonArray.Add(jsonArrayItem);

                }
                var json = JSON.ToJSON(jsonArray);
                return Encoding.UTF8.GetBytes(json);
            }
        }

        protected static string ToJsonString(string source)
        {
            return source?.Replace(@"\", @"\\")
                .Replace(@"'", @"\'")
                .Replace("\"", "\\\"") ?? string.Empty;
        }


        #region consts

        private const string XlsExtension = "xls";
        private const string XlsxExtension = "xlsx";
        private const string JsonExtension = "json";

        #endregion

        #region Utilities

        private IDictionary<string, int> GetColumnNamesAndIndexes(IDataRecord excelReader)
        {
            var result = new Dictionary<string, int>();
            for (var curIndex = 0; curIndex < excelReader.FieldCount; curIndex++)
            {
                if (excelReader[curIndex] == null)
                    continue;

                var header = excelReader.GetString(curIndex);
                if (string.IsNullOrEmpty(header) || string.IsNullOrWhiteSpace(header))
                    continue;
                result[header] = curIndex;
            }
            return result;
        }

        private static IExcelDataReader CreateExcelDataReader(string extension, Stream ms)
        {
            return extension == XlsExtension
                ? ExcelReaderFactory.CreateBinaryReader(ms)
                : ExcelReaderFactory.CreateOpenXmlReader(ms);
        }


        #endregion
    }
}