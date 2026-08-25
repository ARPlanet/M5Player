using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    public class PrefabSource
    {
        protected IPersistentToGameObjectConverterRegistry registry;
        readonly IAssetDataBaseManager _assetDataBaseManager;

        public PrefabSource(IAssetDataBaseManager assetDataBaseManager, IPersistentToGameObjectConverterRegistry persistentToGameObjectConverterRegistry)
        {
            registry = persistentToGameObjectConverterRegistry;
            _assetDataBaseManager = assetDataBaseManager;
        }

        public virtual string Name { get; set; }
        public PersistentPrefab persistent;
        public virtual async Task<GameObject> Load(IAssetLoaderManager assetLoaderManager, bool allowGlobalFallback = false)
        {
            PersistentToObjectDataBase dataBase = new (_assetDataBaseManager, assetLoaderManager, true);
            dataBase.AllowGlobalFallback = allowGlobalFallback;
            // Load Asset
            if (persistent.assetIds != null)
            {
                foreach (string assetId in persistent.assetIds)
                {
                    if (Guid.TryParse(assetId, out Guid guid))
                    {
                        await dataBase.LoadAsset(guid);
                    }
                }
            }
            // Create
            PersistentToPrefabConverter converter = new(registry)
            {
                persistent = persistent
            };
            await converter.CreateObject(dataBase);
            // Mapping Value
            await converter.PersistentToObject(dataBase);
            converter.root.name = Name;
            return converter.root;
        }
    }

    public class PrefabStorage : AssetStorage<AssetMeta>
    {
        public static readonly string[] Extensions = { ".prefab" };
        public override IReadOnlyList<string> SupportedExtensions => Extensions;
        public override Type RuntimeAssetType => typeof(PrefabSource);

        protected IPersistentToGameObjectConverterRegistry Registry { get; }

        public PrefabStorage(IAssetDataBaseManager assetDataBaseManager, IPersistentToGameObjectConverterRegistry registry) 
            : base(assetDataBaseManager)
        {
            Registry = registry;
        }

        public override async Task<object> LoadAssetObjectAsync(string filePath, AssetMeta meta)
        {
            string text = await File.ReadAllTextAsync(filePath);
            PersistentPrefab persistentPrefab = JsonConvert.DeserializeObject<PersistentPrefab>(text);
            return persistentPrefab;
        }

        public override void CreateInstance(Guid guid, object loadedObj, AssetMeta meta)
        {
            if (loadedObj is not PersistentPrefab persistentPrefab) return;
            PrefabSource prefabSource = new(AssetDataBaseManager, Registry)
            {
                Name = Path.GetFileNameWithoutExtension(meta.guid),
                persistent = persistentPrefab
            };

            InstanceContainer instance = AssetDataBaseManager.CreateInstanceContainer(guid);
            instance.Obj = prefabSource;
        }
    }
}
