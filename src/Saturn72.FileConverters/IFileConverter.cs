using System.Collections.Generic;
using System.IO;

namespace Saturn72.FileConverters
{
    public interface IFileConverter
    {
        IEnumerable<FileConversionData> SupportedConversions { get; }

        byte[] Convert(string sourceExtension, string destinationExtension, Stream stream);

    }
}
