namespace Saturn72.FileConverters
{
    public class FileConversionData
    {
        public FileConversionData(string sourceExtension, string destinationExtension)
        {
            SourceExtension = sourceExtension;
            DestinationExtension = destinationExtension;
        }

        public string SourceExtension { get; }
        public string DestinationExtension { get; }
    }
}