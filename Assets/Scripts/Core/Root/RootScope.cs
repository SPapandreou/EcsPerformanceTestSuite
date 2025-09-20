#if !UNITY_EDITOR
using System.IO;
#endif

using Core.Configuration;
using Core.EcsWorld;
using Core.Tests;
using Newtonsoft.Json;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Root
{
    public class RootScope : LifetimeScope
    {
        public TextAsset appConfigJson;
        public string appConfigPath;

        protected override void Configure(IContainerBuilder builder)
        {
#if UNITY_EDITOR
            var appConfig = JsonConvert.DeserializeObject<AppConfig>(appConfigJson.text);
            builder.RegisterInstance(appConfig);
#else
            var appConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(appConfigPath));
            builder.RegisterInstance(appConfig);
#endif

            builder.Register<TestManager>(Lifetime.Singleton);
        }
    }
}