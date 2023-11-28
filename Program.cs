using System.Reflection;
using Shared;

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
    static IConfigurationProvider LoadFromPlugin()
    {
        // load dll from Plugins folder;
        var assemblyFile = Directory.GetFiles(@".\Plugins", "*dll").FirstOrDefault();
        var asm = Assembly.LoadFrom(assemblyFile);

        var provider = asm.GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IConfigurationProvider))).FirstOrDefault();
        return Activator.CreateInstance(provider, @".\config.json") as IConfigurationProvider;
    }

    static void Main()
    {       
        var fileProvider = LoadFromPlugin();
        var appConfigFile = new AppConfig(fileProvider);

        appConfigFile.LogLevel = 3;
        appConfigFile.Timeout = TimeSpan.FromSeconds(10);
        Console.WriteLine($"Log Level: {appConfigFile.LogLevel}");
        Console.WriteLine($"Timeout: {appConfigFile.Timeout.TotalSeconds} seconds");
    }
}