using Microsoft.Extensions.DependencyInjection;

namespace ChatgptSdk.Lib;

static public class Module
{
    /// <summary>
    /// Register the ChatGptSdk with the dependency injection container.
    /// </summary>
    /// <param name="serviceCollection"></param>
    public static void Register(IServiceCollection serviceCollection, ConfigManager configManager)
    {
        serviceCollection.AddSingleton<IChatGptSdk>(sp => new ChatGptSdk(configManager.Build()));
    }
}
