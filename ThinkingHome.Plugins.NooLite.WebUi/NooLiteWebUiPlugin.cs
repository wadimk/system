using System;
using ThinkingHome.Core.Plugins;
using ThinkingHome.Plugins.WebUi.Apps;
using ThinkingHome.Plugins.WebUi.Attributes;

namespace ThinkingHome.Plugins.NooLite.WebUi
{
    [AppSection(SectionType.System, "NooLite", "/static/scripts/web-ui/noolite.js", "ThinkingHome.Plugins.Scripts.NooLite.Resources.noolite.js", SortOrder = 25)]
    [JavaScriptResource("/static/noolite/web-ui/editor.js", "ThinkingHome.Plugins.NooLite.WebUi.Resources.editor.js")]
    [CssResource("/static/noolite/web-ui/styles.css", "ThinkingHome.Plugins.NooLite.WebUi.Resources.styles.css", AutoLoad = true)]

    // templates
    [TemplateResource("/static/noolite/web-ui/list.tpl", "ThinkingHome.Plugins.NooLite.WebUi.Resources.list.tpl")]
    [TemplateResource("/static/noolite/web-ui/list-item.tpl", "ThinkingHome.Plugins.NooLite.WebUi.Resources.list-item.tpl")]
    [TemplateResource("/static/noolite/web-ui/editor.tpl", "ThinkingHome.Plugins.NooLite.WebUi.Resources.editor.tpl")]


    public class NooLiteWebUiPlugin : PluginBase  
    {
        
    }
}
