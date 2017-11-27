﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ThinkingHome.Core.Plugins;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace ThinkingHome.Core.Infrastructure
{
    public class ServiceContext : IServiceContext
    {
        public IConfigurationSection Configuration { get; }
        public IStringLocalizerFactory LocalizerFactory { get; }

        private readonly Dictionary<Type, PluginBase> plugins;

        public ServiceContext(
            IEnumerable<PluginBase> loadedPlugins,
            IConfigurationSection configuration,
            ILoggerFactory loggerFactory,
            IStringLocalizerFactory localizerFactory)
        {
            Configuration = configuration;
            LocalizerFactory = localizerFactory;
            plugins = loadedPlugins.ToDictionary(p => p.GetType());

            foreach (var plugin in plugins.Values)
            {
                var type = plugin.GetType();

                plugin.Context = this;
                plugin.Logger = loggerFactory.CreateLogger(type);
                plugin.Configuration = configuration.GetSection(type.FullName);
                plugin.StringLocalizer = localizerFactory.Create(type);
            }
        }

        public IReadOnlyCollection<PluginBase> GetAllPlugins()
        {
            return new ReadOnlyCollection<PluginBase>(plugins.Values.ToList());
        }

        public T Require<T>() where T : PluginBase
        {
            return plugins[typeof(T)] as T;
        }
    }
}