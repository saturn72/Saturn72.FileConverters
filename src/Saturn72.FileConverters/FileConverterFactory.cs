using System.Collections.Generic;
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
    }
}
