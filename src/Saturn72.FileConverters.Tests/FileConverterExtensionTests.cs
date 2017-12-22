using Moq;
using Shouldly;
using Xunit;

namespace Saturn72.FileConverters.Tests
{
    public class FileConverterExtensionTests
    {
        [Fact]
        public void FileConverterExtension_IsSupported()
        {
            var fc = new Mock<IFileConverter>();
            fc.Object.IsSupported("dd", "sss").ShouldBeFalse();

            fc.Setup(x => x.SupportedConversions).Returns(new[]
            {
                new FileConversionData("a", "b"),
            });

            fc.Object.IsSupported("dd", "sss").ShouldBeFalse();

            fc.Object.IsSupported("a", "b").ShouldBeTrue();
        }
    }
}
