using System;
using System.Configuration;
using System.IO;


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



namespace Reflection
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}