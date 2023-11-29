using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reflection
{
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
}
