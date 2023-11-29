using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
namespace Reflection;


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


class Program
{
    static IConfigurationProvider[] LoadFromPlugin()
    {
        var plugins = new List<IConfigurationProvider>();
        foreach (var file in Directory.GetFiles(@".\Plugins", "*.dll"))
        {
            try
            {
                var assembly = Assembly.LoadFrom(Directory.GetCurrentDirectory() + file);

                foreach (var type in assembly.GetTypes())
                {
                    if (type.GetInterfaces().Contains(typeof(IConfigurationProvider)))
                    {
                        var plug = Activator.CreateInstance(type) as IConfigurationProvider;
                        plugins.Add(plug);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while loading plugin");
            }
        }


        return plugins.ToArray();

    }
    static void Main()
    {
        var fileProvider = LoadFromPlugin();

        foreach (var provider in fileProvider)
        {
            Console.WriteLine(provider);
            var appConfigFile = new AppConfig(provider);

            appConfigFile.LogLevel = 3;
            appConfigFile.Timeout = TimeSpan.FromSeconds(10);
            Console.WriteLine($"Log Level: {appConfigFile.LogLevel}");
            Console.WriteLine($"Timeout: {appConfigFile.Timeout.TotalSeconds} seconds");
        }

    }
}