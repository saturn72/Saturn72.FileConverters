using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Saturn72.FileConverters
{
    public class FileConverterFactory
    {
        private readonly IEnumerable<IFileConverter> _fileConverters;

        public FileConverterFactory(IEnumerable<IFileConverter> fileConverters)
        {
            _fileConverters = fileConverters;
        }

        public virtual IFileConverter GetFileConverter(string sourceExtension, string destinationExtension)
        {
            return _fileConverters?.FirstOrDefault(fc => fc.IsSupported(sourceExtension, destinationExtension));
        }

        public virtual byte[] Convert(string sourceFormat, string destinationFormat, byte[] bytes)
        {
            var fileConverter = GetFileConverter(sourceFormat, destinationFormat);
            if (fileConverter == null)
                throw new NotSupportedException(
                    string.Format("The conversion is not supported. Source: {0} Destination: {1}", sourceFormat,
                        destinationFormat));
            using (var ms = new MemoryStream(bytes))
            {
                return fileConverter.Convert(sourceFormat, destinationFormat, ms);
            }
        }
    }
}
