using CSharpExtensions.Lib.Railway.Profiles;

namespace CSharpExtensions.Lib.Railway.Configurations;

public static class RailwayConfiguration
{
    static RailwayConfiguration()
    {
        ActionResultProfileSettings = new ActionResultProfileSettings();
    }

    internal static ActionResultProfileSettings ActionResultProfileSettings { get; private set; }

    public static void Setup(Action<ActionResultProfileSettings> setupFunc)
    {
        var settingsBuilder = new ActionResultProfileSettings();
        setupFunc(settingsBuilder);

        ActionResultProfileSettings = settingsBuilder;
    }

    public static IActionResultProfile GetCurrentProfile()
    {
        return ActionResultProfileSettings.CurrentProfile;
    }
}
