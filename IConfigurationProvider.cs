using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reflection
{
    public interface IConfigurationProvider
    {
        void SaveSetting(string key, string value);
        string LoadSetting(string key);
    }
}
