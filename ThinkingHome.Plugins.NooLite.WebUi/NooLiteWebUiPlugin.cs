using System;
using ThinkingHome.Core.Plugins;
using ThinkingHome.Plugins.WebServer.Attributes;
using ThinkingHome.Plugins.WebUi.Apps;
using ThinkingHome.Plugins.WebUi.Attributes;

namespace ThinkingHome.Plugins.NooLite.WebUi
{
    [AppSection(SectionType.System, "NooLite", "/static/scripts/web-ui/noolite.js", "ThinkingHome.Plugins.Scripts.NooLite.Resources.noolite.js", SortOrder = 25)]
    [CssResource("/static/noolite/web-ui/styles.css", "ThinkingHome.Plugins.NooLite.WebUi.Resources.styles.css", AutoLoad = true)]

    // templates
    [TemplateResource("/static/noolite/web-ui/list.tpl", "ThinkingHome.Plugins.NooLite.WebUi.Resources.list.tpl")]
    [TemplateResource("/static/noolite/web-ui/list-item.tpl", "ThinkingHome.Plugins.NooLite.WebUi.Resources.list-item.tpl")]

    // i18n
    [HttpLocalizationResource("/static/noolite/lang.json")]

    public class NooLiteWebUiPlugin : PluginBase  
    {
        
    }
}
