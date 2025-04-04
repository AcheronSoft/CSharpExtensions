using CSharpExtensions.Lib.Extensions;
using CSharpExtensions.Tests.Base;
using FluentAssertions;

namespace CSharpExtensions.Tests.UnitTests;

[Collection(nameof(SharedFixture))]
public class JsonTests
{
    public JsonTests(SharedFixture sharedFixture)
    {
    }

    [Fact]
    public void JsonOptionsTest()
    {
        var row = new
        {
            BranchId = 7777,
            BranchUpdateTime = DateTime.Now,
            NameEng = "SaveAsJson",
            NameRus = "ТЕСТ7777"
        };

        var json = row.ToJson(x =>
        {
            x.PropertyNamingPolicy = JsonExtensions.LowerCaseNamingPolicy;
            x.Encoder = JsonExtensions.CyrillicEncoder;
        });

        json.Should().Contain("branchid");
        json.Should().Contain("ТЕСТ7777");
    }
}
