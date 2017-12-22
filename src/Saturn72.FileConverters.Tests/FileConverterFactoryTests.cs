using Moq;
using Shouldly;
using Xunit;

namespace Saturn72.FileConverters.Tests
{
    public class FileConverterFactoryTests
    {
        [Fact]
        public void FileConverterFactory_GetFileConverter()
        {
            const string srcExt = "src";
            const string destExt = "dest";

            var fcf1 = new FileConverterFactory(null);
            fcf1.GetFileConverter(srcExt, destExt).ShouldBeNull();

            var fcs = new Mock<IFileConverter>();
            var fcf2 = new FileConverterFactory(new[]{fcs.Object});
            fcf2.GetFileConverter(srcExt, destExt).ShouldBeNull();

            fcs.Setup(f => f.SupportedConversions).Returns(new[] {new FileConversionData(srcExt, destExt)});
            fcf2.GetFileConverter(srcExt, destExt).ShouldBe(fcs.Object);

        }
    }
}
