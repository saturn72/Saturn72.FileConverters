using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ExcelDataReader;
using Saturn72.Extensions;
using ServiceStack.Text;

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

        private static readonly IEnumerable<FileConversionData> _supportedConversions = new[]
        {
            new FileConversionData(XlsExtension, JsonExtension),
            new FileConversionData(XlsxExtension, JsonExtension)
        };

        public IEnumerable<FileConversionData> SupportedConversions => _supportedConversions;

        public byte[] Convert(string sourceExtension, string destinationExtension, byte[] bytes, object data)
        {
            Guard.NotNull(bytes);

            if (!this.IsSupported(sourceExtension, destinationExtension))
                throw new NotSupportedException(
                    string.Format(
                        "The required conversion is not supported (From {0} file extension sto to {1} file extension)",
                        sourceExtension, destinationExtension));
            using (var stream = new MemoryStream(bytes))
            using (var excelReader = CreateExcelDataReader(sourceExtension, stream))
            {
                if (!excelReader.Read())
                    return new byte[] { };

                var columNamesAndIndexes = GetColumnNamesAndIndexes(excelReader);
                var jsonArray = new List<IDictionary<string, object>>();

                while (excelReader.Read())
                {
                    var jsonArrayItem = new Dictionary<string, object>();


                    foreach (var cnni in columNamesAndIndexes)
                    {
                        var key = ToJsonConvertFunc(typeof(string))(cnni.Key) as string;
                        var valueType = excelReader.GetFieldType(cnni.Value);
                        var value = ToJsonConvertFunc(valueType)(excelReader.GetValue(cnni.Value));
                        jsonArrayItem[key] = value;

                    }
                    jsonArray.Add(jsonArrayItem);

                }
                var json = JsonSerializer.SerializeToString(jsonArray);
                return Encoding.UTF8.GetBytes(json);
            }
        }

        protected static Func<object, object> ToJsonConvertFunc(Type t) => t == null ? obj => string.Empty : ToJsonDictionary.First(x => x.Key.Contains(t)).Value;

        protected static readonly IDictionary<IEnumerable<Type>, Func<object, object>> ToJsonDictionary = new Dictionary<IEnumerable<Type>, Func<object, object>>
        {
            {
                new []{typeof(string), typeof(char)},
                str => (str as string)?.Replace(@"\", @"\\")
                           .Replace(@"'", @"\'")
                           .Replace("\"", "\\\"") ?? string.Empty
            },
            {
                new[]{typeof(bool), typeof(byte),}, b=> System.Convert.ToBoolean(b)
            },
            {
                new[]{typeof(DateTime),
                    typeof(decimal),
                    typeof(double),
                    typeof(float),
                    typeof(Guid),
                    typeof(short),
                    typeof(int),
                    typeof(long)}, d=>d
            },
        };


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

        private static IExcelDataReader CreateExcelDataReader(string extension, Stream stream)
        {

            return extension == XlsExtension
                  ? ExcelReaderFactory.CreateBinaryReader(stream)
                  : ExcelReaderFactory.CreateOpenXmlReader(stream);

        }


        #endregion
    }
}