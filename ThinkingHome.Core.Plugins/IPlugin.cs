using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ThinkingHome.Core.Plugins
{
    public interface IPlugin
    {
        void InitPlugin();
        void StartPlugin();
        void StopPlugin();

        IStringLocalizer StringLocalizer { get; set; }
        IConfigurationSection Configuration { get; set; }
        ILogger Logger { get; set; }

        void SafeInvoke<T>(IEnumerable<T> handlers, Action<T> action, bool async = false);
        void SafeInvoke<T>(T handler, Action<T> action, bool async = false);
    }
}