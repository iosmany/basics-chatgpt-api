namespace ChatgptSdk.Lib;

using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.IO;

public class ConfigManager
{
    private readonly IConfiguration _configuration;
    private readonly string _configFilePath;

    public ConfigManager(string configFilePath = "appsettings.json")
    {
        _configFilePath = configFilePath;

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) 
            .AddJsonFile(configFilePath, optional: true, reloadOnChange: true)
            .AddEnvironmentVariables(); 
            //.AddCommandLine(Environment.GetCommandLineArgs()); 

        _configuration = builder.Build();
    }

    internal AppConfig Build()
         => _configuration.BuildAppConfig(); // Bind the settings to the AppConfig object

    public string GetString(string key, string defaultValue = null)
    {
        return _configuration[key] ?? defaultValue;
    }

    public int GetInt(string key, int defaultValue = 0)
    {
        if (int.TryParse(_configuration[key], out var result))
            return result;
        return defaultValue;
    }

    public bool GetBool(string key, bool defaultValue = false)
    {
        if (bool.TryParse(_configuration[key], out var result))
            return result;
        return defaultValue;
    }

    public string[] GetStringList(string key, string[] defaultValues = null)
    {
        var value = _configuration[key];
        return string.IsNullOrEmpty(value) ? defaultValues : value.Split(',');
    }

    public bool Exists(string key)
        => _configuration[key] != null;

    public T GetOrDefault<T>(string key, T defaultValue)
    {
        var value = _configuration[key];
        if (value == null)
            return defaultValue;

        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }
}

static class ConfigExtensions
{
    public static AppConfig BuildAppConfig(this IConfiguration configuration)
    {
        var apiKey = configuration["ApiKey"] ?? throw new ArgumentNullException("ApiKey must have a value");
        var databaseConnection = configuration["DatabaseConnection"] ?? throw new ArgumentNullException("DatabaseConnection must have a value");
        var model= configuration["GptModel"] ?? throw new ArgumentNullException("GPT model must have a value");
        if (int.TryParse(configuration["MaxRetryAttempts"], out var retryPolicy))
            throw new InvalidOperationException("MaxRetryAttempts should be convertible to Int32");

        return new AppConfig(apiKey, databaseConnection, model, retryPolicy);
    }
    
}

sealed class AppConfig
{
    public string ApiKey { get;  }
    public string DatabaseConnection { get; }
    public int MaxRetryAttempts { get;  }
    public string Model { get; }

    public AppConfig(string apiKey, string databaseConnection, string model, int maxRetryAttempts)
    {
        ApiKey = apiKey;
        DatabaseConnection = databaseConnection;
        MaxRetryAttempts = maxRetryAttempts;
    }
}