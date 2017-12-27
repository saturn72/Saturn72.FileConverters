using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using fastJSON;
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

            var delimiter = GetDelimiter(data);
            using (var reader = new StringReader(Encoding.UTF8.GetString(bytes)))
            {
                var headers = CsvLineToArray(reader.ReadLine(), delimiter);
                var jsonArray = new List<IDictionary<string, object>>();

                string curLine;
                while ((curLine = reader.ReadLine()).NotNull())
                {
                    var jsonValues = CsvLineToArray(curLine, delimiter);
                    var jsonArrayItem = new Dictionary<string, object>();
                    for (int i = 0; i < headers.Count(); i++)
                        jsonArrayItem[headers.ElementAt(i)] = ToJsonObject(jsonValues.ElementAt(i));
                    jsonArray.Add(jsonArrayItem);
                }

                var json = JSON.ToJSON(jsonArray);
                return Encoding.UTF8.GetBytes(json);
            }
        }

        private object ToJsonObject(string value)
        {
            if (value.Length == 1 && char.TryParse(value, out char chrRes))
                return chrRes;

            if (bool.TryParse(value, out bool boolRes))
                return boolRes;

            if (short.TryParse(value, out short shortRes))
                return shortRes;

            if (int.TryParse(value, out int intRes))
                return intRes;

            if (long.TryParse(value, out long longRes))
                return longRes;

            if (double.TryParse(value, out double doubleRes))
                return doubleRes;

            if (float.TryParse(value, out float floatRes))
                return floatRes;

            if (Guid.TryParse(value, out Guid guidRes))
                return guidRes;

            if (DateTime.TryParse(value, out DateTime dateTimeRes))
                return dateTimeRes;

            return value;
        }

        private static IEnumerable<string> CsvLineToArray(string line, char delimiter)
        {
            return line.Split(delimiter).Select(h => h.Trim()).ToArray();
        }

        #region Utilities

        private static char GetDelimiter(object data)
        {
            return (char)(data?.GetType().GetProperty("delimiter")?.GetValue(data, null) ?? ',');
        }

        #endregion
    }
}
