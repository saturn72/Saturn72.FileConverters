using System.Linq;

namespace Saturn72.FileConverters
{
    public static class FileConverterExtensions
    {
        public static bool IsSupported(this IFileConverter fileConverter, string sourceExtension,
            string destinationExtension)
        {
            return fileConverter.SupportedConversions?.Any(c => c.SourceExtension == sourceExtension &&
                                          c.DestinationExtension == destinationExtension) ?? false;
        }
    }
}
