using System.Collections.Generic;

namespace Saturn72.FileConverters
{
    public interface IFileConverter
    {
        IEnumerable<FileConversionData> SupportedConversions { get; }

        byte[] Convert(string sourceExtension, string destinationExtension, byte[] bytes, object data);

    }
}
