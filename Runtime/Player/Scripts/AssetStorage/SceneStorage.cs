using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    public class SceneStorage : AssetStorage<AssetMeta>
    {
        public static readonly string[] Extensions = { ".scene" };
        public override IReadOnlyList<string> SupportedExtensions => Extensions;
        public override Type RuntimeAssetType => typeof(SceneData);

        protected IPersistentToGameObjectConverterRegistry Registry { get; }

        public SceneStorage(IAssetDataBaseManager assetDataBaseManager, IPersistentToGameObjectConverterRegistry registry) 
            : base(assetDataBaseManager)
        {
            Registry = registry;
        }

        public override async Task<object> LoadAssetObjectAsync(string filePath, AssetMeta meta)
        {
            string text = await File.ReadAllTextAsync(filePath);
            PersistentScene persistentScene = JsonConvert.DeserializeObject<PersistentScene>(text);
            return persistentScene;
        }

        public override void CreateInstance(Guid guid, object loadedObj, AssetMeta meta)
        {
            if (loadedObj is not PersistentScene persistentScene) return;
            SceneData sceneData = new(AssetDataBaseManager, Registry)
            {
                PersistentScene = persistentScene,
                Name = meta.guid,
                Path = ""
            };
            InstanceContainer instance = AssetDataBaseManager.CreateInstanceContainer(guid);
            instance.Obj = sceneData;
        }
    }
}
