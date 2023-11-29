using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reflection
{
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
}
