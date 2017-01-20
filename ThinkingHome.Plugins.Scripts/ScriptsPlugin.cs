﻿using System;
using System.Linq;
using Jint;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ThinkingHome.Core.Plugins;
using ThinkingHome.Core.Plugins.Utils;
using ThinkingHome.Plugins.Database;
using ThinkingHome.Plugins.Scripts.Attributes;
using ThinkingHome.Plugins.Scripts.Internal;
using ThinkingHome.Plugins.Scripts.Model;

namespace ThinkingHome.Plugins.Scripts
{
    public class ScriptsPlugin : PluginBase
    {
        private static readonly Engine engine = new Engine();

        private readonly InternalDictionary<Delegate> methods = new InternalDictionary<Delegate>();

        public override void InitPlugin(IConfigurationSection config)
        {
            // регистрируем методы плагинов
            foreach (var plugin in Context.GetAllPlugins())
            {
                var pluginTypeName = plugin.GetType().FullName;

                foreach (var mi in plugin.FindMethodsByAttribute<ScriptCommandAttribute, Delegate>())
                {
                    Logger.Info($"register script method: \"{mi.MetaData.Alias}\" ({pluginTypeName})");
                    methods.Register(mi.MetaData.Alias, mi.Method);
                }
            }

            // создаем объект host
            var host = new
            {
                scripts = new ScriptMethodContainer<Func<object[], object>>(CreateScriptDelegateByName),
                api = new ScriptMethodContainer<Delegate>(GetMethodDelegate),
                log = new ScriptLogger(Logger)
            };

            engine.SetValue("host", host);
        }

        [DbModelBuilder]
        public void InitModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserScript>(cfg => cfg.ToTable("Scripts_UserScript"));
            modelBuilder.Entity<ScriptEventHandler>(cfg => cfg.ToTable("Scripts_EventHandler"));
        }

        #region public API

        public object ExecuteScript(string body, params object[] args)
        {
            return CreateScriptDelegate(null, body)(args);
        }

        public object ExecuteScript(UserScript script, params object[] args)
        {
            return CreateScriptDelegate(script.Name, script.Body)(args);
        }

        public object ExecuteScriptByName(string name, params object[] args)
        {
            return CreateScriptDelegateByName(name)(args);
        }

//        public void RaiseScriptEvent(string alias, params object[] args)
//        {
//            using (var session = Context.Require<DatabasePlugin>().OpenSession())
//            {
//                var scripts = session.Set<UserScript>()
//                    .Where(s => s.Name == alias)
//            }
//        }

        #endregion

        #region private

        private Func<object[], object> CreateScriptDelegate(string name, string body)
        {
            return new ScriptContext(name, body, engine, Logger).Execute;
        }

        private Func<object[], object> CreateScriptDelegateByName(string name)
        {
            try
            {
                using (var session = Context.Require<DatabasePlugin>().OpenSession())
                {
                    var script = session.Set<UserScript>().Single(s => s.Name == name);
                    return CreateScriptDelegate(script.Name, script.Body);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Can't find the script '{name}'");
                return null;
            }
        }

        private Delegate GetMethodDelegate(string name)
        {
            try
            {
                return methods[name];
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Can't find the method '{name}'");
                return null;
            }
        }

        #endregion
    }
}
