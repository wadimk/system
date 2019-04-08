﻿using ThinkingHome.Core.Plugins;
using ThinkingHome.Core.Plugins.Utils;
using ThinkingHome.Plugins.Cron.Model;
using ThinkingHome.Plugins.Database;
using ThinkingHome.Plugins.WebServer.Attributes;
using ThinkingHome.Plugins.WebServer.Handlers;

namespace ThinkingHome.Plugins.Database.Backup
{
    public class DatabaseBackup : PluginBase
    {
        public override void InitPlugin()
        {
            base.InitPlugin();
        }


        [HttpJsonDynamicResource("/dynamic/web-ui/backup.json", IsCached = false)]
        public object BackupData(HttpRequestParams request)
        {
            var database = Context.Require<DatabasePlugin>();

            if (!database.IsInitialized)
            {
                return null;
            }

            using (var session = database.OpenSession())
            {
                var tasks = session.Set<CronTask>();
                return tasks.ToJson();

            }            
        }
    }


}
