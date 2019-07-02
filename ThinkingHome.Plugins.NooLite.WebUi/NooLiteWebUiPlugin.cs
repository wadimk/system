using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using ThinkingHome.Core.Plugins;
using ThinkingHome.Plugins.Timer;
using ThinkingHome.Plugins.WebServer;
using ThinkingHome.Plugins.WebServer.Attributes;
using ThinkingHome.Plugins.WebServer.Handlers;
using ThinkingHome.Plugins.WebServer.Messages;
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

        public override void StartPlugin()
        {
            
        }

        public override void StopPlugin()
        {
            
        }

        [WebApiMethod("/api/noolite/web-api/list")]
        public object GetTaskList(HttpRequestParams request)
        {
            var list = new List<NooliteChannel>();
            
            list.Add(new NooliteChannel() { Channel = 0 });
            list.Add(new NooliteChannel() { Channel = 1 });
            list.Add(new NooliteChannel() { Channel = 2 });
            list.Add(new NooliteChannel() { Channel = 3 });
            list.Add(new NooliteChannel() { Channel = 4 });
            list.Add(new NooliteChannel() { Channel = 5 });
            list.Add(new NooliteChannel() { Channel = 6 });
            list.Add(new NooliteChannel() { Channel = 7 });
            list.Add(new NooliteChannel() { Channel = 8 });


            return list;
        }

        [WebApiMethod("/api/noolite/web-api/channel")]
        public object GetResult(HttpRequestParams request)
        {
            var ch = request.GetRequiredByte("ch");
            var command = request.GetRequiredString("command").ToLower();
            return SendCommand(ch, command);
        }

        [HubMessageHandler("noolite:command")]
        public void NooliteMessageHandler(Guid msgId, DateTime timestamp, string channel, object data)
        {
            Logger.LogInformation("{0}:{1}:{2}:{3}", msgId, timestamp, channel, data);
            var array = data.ToString().Split('-');
            SendCommand(Convert.ToByte(array[1]), array[0]);
        }

        private string SendCommand(byte ch, string command)
        {
            var noolite = Context.Require<NooLitePlugin>();
            var adapter = noolite.Open(false);

            switch (command)
            {
                case "on":
                    adapter.On(ch);
                    break;

                case "off":
                    adapter.Off(ch);
                    break;

                case "bind":
                    adapter.Bind(ch);
                    break;

                case "unbind":
                    adapter.UnBind(ch);
                    break;

                default:
                    return "error, command is not supported";
            }


            return $"{ch}: {command}";

        }


    }

    public class CommandResult
    {

    }

    public class NooliteChannel
    {
        public int Channel { get; set; }

        public string Name => $"Channel {Channel}";
    }
}
