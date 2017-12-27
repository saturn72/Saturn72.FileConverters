using System;
using System.Collections.Generic;
using System.Linq;
using Saturn72.Extensions;

namespace Saturn72.FileConverters
{
    public class CsvToJsonFileConverter : IFileConverter
    {
        #region Consts

        private const string CsvExtension = "csv";
        private const string JsonExtension = "json";

        #endregion

        #region Static members

        private static readonly IEnumerable<FileConversionData> _supportedConversions = new[]
        {
            new FileConversionData(CsvExtension, JsonExtension),
        };

        #endregion

        public IEnumerable<FileConversionData> SupportedConversions => _supportedConversions;

        public byte[] Convert(string sourceExtension, string destinationExtension, byte[] bytes, object data)
        {
            Guard.NotNull(bytes);

            if (!this.IsSupported(sourceExtension, destinationExtension))
                throw new NotSupportedException(
                    string.Format(
                        "The required conversion is not supported (From {0} file extension sto to {1} file extension)",
                        sourceExtension, destinationExtension));

            if (!bytes.Any())
                return new byte[] { };
                throw new System.NotImplementedException();

            /*
             

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
                var json = JSON.ToJSON(jsonArray);
                return Encoding.UTF8.GetBytes(json);
            }
            */
        }
    }
}
