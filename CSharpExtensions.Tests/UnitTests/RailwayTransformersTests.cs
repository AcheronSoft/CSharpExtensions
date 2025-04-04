using CSharpExtensions.Lib.Railway;
using CSharpExtensions.Lib.Railway.Contexts;
using CSharpExtensions.Lib.Railway.Profiles;
using CSharpExtensions.Lib.Railway.Transformers;
using CSharpExtensions.Tests.Base;
using Moq;

namespace CSharpExtensions.Tests.UnitTests;

[Collection(nameof(SharedFixture))]
public class RailwayTransformersTests
{
    private readonly Mock<IActionResultProfile> _profileMock = new();

    [Fact]
    public void Transform_CallsCorrectProfileMethod()
    {
        // Arrange
        var transformer = new ActionResultTransformer();
        var successResult = Result.Success();

        // Act
        transformer.Transform(successResult, _profileMock.Object);

        // Assert
        _profileMock.Verify(p => p.TransformToSuccessActionResult(It.IsAny<ActionResultProfileSuccessContext>()));
    }
}
