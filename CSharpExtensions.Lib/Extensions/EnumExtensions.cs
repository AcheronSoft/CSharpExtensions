using CSharpExtensions.Lib.Enums;

namespace CSharpExtensions.Lib.Extensions;

public static class EnumExtensions
{
    public static bool ToBool(this IsNullable isNullable)
    {
        return isNullable == IsNullable.True;
    }
}
