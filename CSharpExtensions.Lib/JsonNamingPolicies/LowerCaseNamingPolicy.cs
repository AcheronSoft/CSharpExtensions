using System.Text.Json;
using CSharpExtensions.Lib.Extensions;

namespace CSharpExtensions.Lib.JsonNamingPolicies;

public class LowerCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return StringExtensions.IsNullOrEmptyOrWhiteSpace(name) ? name : name.ToLower();
    }
}
