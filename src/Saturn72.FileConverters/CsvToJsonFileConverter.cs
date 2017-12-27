using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using fastJSON;
using Saturn72.Extensions;

namespace Saturn72.FileConverters
{
    public class CsvToJsonFileConverter : IFileConverter
    {
        #region Consts

        private const string CsvExtension = "csv";
        private const string JsonExtension = "json";
        private const char Qualifier = '\\';
        private static readonly string QualifierString = new string(new[] { Qualifier });

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
                    for (var i = 0; i < headers.Count(); i++)
                        jsonArrayItem[headers.ElementAt(i)] = ToJsonObject(jsonValues.ElementAt(i));
                    jsonArray.Add(jsonArrayItem);
                }

                var json = JSON.ToJSON(jsonArray);
                return Encoding.ASCII.GetBytes(json);
            }
        }

        private object ToJsonObject(string value)
        {
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
            if (char.TryParse(value, out char chrRes))
                return chrRes;
            return value.Replace(QualifierString, string.Empty);
        }

        private static IEnumerable<string> CsvLineToArray(string line, char delimiter)
        {
            var result = new List<string>();
            var wordStartIndex = 0;
            var charBefore = default(char);

            for (var curCharIndex = 0; curCharIndex < line.Length; curCharIndex++)
            {
                var curChar = line[curCharIndex];
                if (curCharIndex == line.Length-1 ||
                    (curChar == delimiter && charBefore != default(char) && charBefore != Qualifier))
                {
                    var onLastWorkAddition = curCharIndex == line.Length - 1 ? 1 : 0;
                    var value = line
                        .Substring(wordStartIndex, curCharIndex - wordStartIndex + onLastWorkAddition)
                        .Trim();
                    result.Add(value);
                    wordStartIndex = curCharIndex + 1;
                    continue;
                }
                charBefore = curChar;
            }
            return result;
        }

        #region Utilities

        private static char GetDelimiter(object data)
        {
            return (char)(data?.GetType().GetProperty("delimiter")?.GetValue(data, null) ?? ',');
        }

        #endregion
    }
}
