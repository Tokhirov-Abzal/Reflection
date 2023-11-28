using System;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;


[AttributeUsage(AttributeTargets.Property)]
public class ConfigurationItemAttribute : Attribute
{
    public string SettingName { get; }
    public string ProviderType { get; }

    public ConfigurationItemAttribute(string settingName, string providerType)
    {
        SettingName = settingName;
        ProviderType = providerType;
    }
}


public interface IConfigurationProvider
{
    void SaveSetting(string key, string value);
    string LoadSetting(string key);
}


public class FileConfigurationProvider : IConfigurationProvider
{
    private readonly string filePath;

    public FileConfigurationProvider(string filePath)
    {
        this.filePath = filePath;
    }

    public void SaveSetting(string key, string value)
    {
        var settings = File.Exists(filePath) ? JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(filePath)) : new Dictionary<string, string>();
        settings[key] = value;
        File.WriteAllText(filePath, JsonConvert.SerializeObject(settings));
    }

    public string LoadSetting(string key)
    {
        if (File.Exists(filePath))
        {
            var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(filePath));
            if (settings.TryGetValue(key, out var value))
            {
                return value;
            }
        }
        return null;
    }
}

public class ConfigurationManagerConfigurationProvider : IConfigurationProvider
{
    public void SaveSetting(string key, string value)
    {
        ConfigurationManager.AppSettings[key] = value;
        ConfigurationManager.RefreshSection("appSettings");
    }

    public string LoadSetting(string key)
    {
        return ConfigurationManager.AppSettings[key];
    }
}

public class ConfigurationComponentBase
{
    private readonly IConfigurationProvider provider;

    public ConfigurationComponentBase(IConfigurationProvider provider)
    {
        this.provider = provider;
    }

    protected T GetSetting<T>(string key)
    {
        var value = provider.LoadSetting(key);
        return (value != null) ? (T)Convert.ChangeType(value, typeof(T)) : default(T);
    }

    protected void SaveSetting(string key, string value)
    {
        provider.SaveSetting(key, value);
    }
}


public class AppConfig : ConfigurationComponentBase
{
    [ConfigurationItemAttribute("LogLevel", "File")]
    public int LogLevel
    {
        get => GetSetting<int>("LogLevel");
        set => SaveSetting("LogLevel", value.ToString());
    }

    [ConfigurationItemAttribute("Timeout", "ConfigurationManager")]
    public TimeSpan Timeout
    {
        get => TimeSpan.FromMilliseconds(GetSetting<double>("Timeout"));
        set => SaveSetting("Timeout", value.TotalMilliseconds.ToString());
    }

    public AppConfig(IConfigurationProvider provider) : base(provider)
    {
    }
}

class Program
{
    static void Main()
    {
        var fileProvider = new FileConfigurationProvider("/config.json");
        var appConfigFile = new AppConfig(fileProvider);

        appConfigFile.LogLevel = 3;
        Console.WriteLine($"Log Level: {appConfigFile.LogLevel}");

        var configManagerProvider = new ConfigurationManagerConfigurationProvider();
        var appConfigManager = new AppConfig(configManagerProvider);

        appConfigManager.Timeout = TimeSpan.FromSeconds(10);
        Console.WriteLine($"Timeout: {appConfigManager.Timeout.TotalSeconds} seconds");
    }
}