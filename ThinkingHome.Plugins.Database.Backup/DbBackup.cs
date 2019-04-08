﻿using System;
using System.Collections.Generic;
using System.Linq;
using ThinkingHome.Core.Plugins;
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
                var entityTypes = session.Model.GetEntityTypes().Select(t => t.ClrType).ToList();

                foreach (var entityType in entityTypes)
                {
                    
                }

                var list = new List<Object>();
                var tasks = session.Set<CronTask>();

               list.AddRange(tasks);


                return list.ToJson();

            }            
        }
    }


}
