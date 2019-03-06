using System;
using System.Collections.Generic;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using ThinkingHome.Core.Plugins;
using ThinkingHome.Plugins.WebServer.Attributes;
using ThinkingHome.Plugins.WebServer.Handlers;
using ThinkingHome.Plugins.WebUi.Apps;
using ThinkingHome.Plugins.WebUi.Attributes;

namespace ThinkingHome.Plugins.NooLite.WebUi
{
    [AppSection(SectionType.System, "NooLite", "/static/scripts/web-ui/noolite.js", "ThinkingHome.Plugins.NooLite.WebUi.Resources.noolite.js", SortOrder = 25)]
    [CssResource("/static/noolite/web-ui/styles.css", "ThinkingHome.Plugins.NooLite.WebUi.Resources.styles.css", AutoLoad = true)]

    // templates
    [TemplateResource("/static/noolite/web-ui/list.tpl", "ThinkingHome.Plugins.NooLite.WebUi.Resources.list.tpl")]
    [TemplateResource("/static/noolite/web-ui/list-item.tpl", "ThinkingHome.Plugins.NooLite.WebUi.Resources.list-item.tpl")]

    // i18n
    [HttpLocalizationResource("/static/noolite/lang.json")]

    public class NooLiteWebUiPlugin : PluginBase  
    {
        public override void InitPlugin()
        {
            Logger.LogInformation($"init NooLite.WebUi plugin {Guid.NewGuid()}");
        }

        [WebApiMethod("/api/noolite/web-api/list")]
        public object GetTaskList(HttpRequestParams request)
        {
            var noolite = Context.Require<NooLitePlugin>();
            var list = new List<NooliteChannel>();
            
            list.Add(new NooliteChannel() { Channel = 0 });
            list.Add(new NooliteChannel() { Channel = 1 });

            return list;
        }

    }

    public class NooliteChannel
    {
        public int Channel { get; set; }

        public string Name => $"Channel {Channel}";
    }
}
