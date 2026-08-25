using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GLTFast;
using UnityEngine;

namespace Module5.Player
{
    [System.Serializable]
    public class GltfMeta : AssetMeta
    {
        //public NameImportMethod nameImportMethod = NameImportMethod.OriginalUnique;
        //public AnimationMethod animationMethod = AnimationMethod.Legacy;
        public bool generateMipMaps = true;
        public int anisotropicFilterLevel = 1;

        public List<SubAssetInfo> bufferInfos = new(); // .bin
        public List<SubAssetInfo> materialInfos = new();
        public List<SubAssetInfo> textureInfos = new();
        public List<SubAssetInfo> meshInfos = new();
        public List<SubAssetInfo> animationInfos = new();
    }

    public class GltfStorage : AssetStorage<GltfMeta>
    {
        public static readonly string[] Extensions = { ".gltf", ".glb" };
        public override IReadOnlyList<string> SupportedExtensions => Extensions;
        public override Type RuntimeAssetType => typeof(GltfImportData);

        protected readonly GltfImporter gltfImporter = new();

        public GltfStorage(IAssetDataBaseManager assetDataBaseManager) : base(assetDataBaseManager) { }

        public override void DeserializeMeta(AssetMeta meta)
        {
            base.DeserializeMeta(meta);
            if (meta is GltfMeta gltfMeta)
            {
                DeserializeSubAssets(gltfMeta.bufferInfos);
                DeserializeSubAssets(gltfMeta.materialInfos);
                DeserializeSubAssets(gltfMeta.textureInfos);
                DeserializeSubAssets(gltfMeta.meshInfos);
                DeserializeSubAssets(gltfMeta.animationInfos);
            }
        }

        protected void DeserializeSubAssets(List<SubAssetInfo> list)
        {
            if (list == null) return;
            foreach (var sub in list)
            {
                if (sub != null && Guid.TryParse(sub.guid, out Guid parsedGuid))
                {
                    sub.m_Guid = parsedGuid;
                }
            }
        }

        public override async Task<object> LoadAssetObjectAsync(string filePath, AssetMeta meta)
        {
            GltfMeta gltfMeta = meta as GltfMeta ?? new GltfMeta();
            ImportSettings importSettings = new()
            {
                NodeNameMethod = NameImportMethod.OriginalUnique,
                AnimationMethod = AnimationMethod.Legacy,
                GenerateMipMaps = gltfMeta.generateMipMaps,
                AnisotropicFilterLevel = gltfMeta.anisotropicFilterLevel,
            };

            GltfImportData gltfImportData = await gltfImporter.ImportAsync(new Uri(filePath, UriKind.Absolute), importSettings);
            if (gltfImportData?.gltfImport != null && !gltfImportData.gltfImport.LoadingError)
            {
                return gltfImportData;
            }
            return null;
        }

        public override async Task<AssetItem> CreateAssetItemAsync(string filePath, AssetMeta meta)
        {
            AssetItem assetItem = await base.CreateAssetItemAsync(filePath, meta);
            GltfMeta gltfMeta = meta as GltfMeta;
            if (gltfMeta != null)
            {
                DeserializeSubAssetGuids(gltfMeta);

                if (gltfMeta.materialInfos != null)
                {
                    foreach (SubAssetInfo subData in gltfMeta.materialInfos)
                    {
                        if (subData.m_Guid == Guid.Empty) continue;

                        AssetItem item = AssetDataBaseManager.CreateAssetItem(subData.m_Guid);
                        item.AssetType = typeof(Material);
                        item.Name = subData.name;
                        item.FullPath = filePath;
                    }
                }

                if (gltfMeta.textureInfos != null)
                {
                    foreach (SubAssetInfo subData in gltfMeta.textureInfos)
                    {
                        if (subData.m_Guid == Guid.Empty) continue;

                        AssetItem item = AssetDataBaseManager.CreateAssetItem(subData.m_Guid);
                        item.AssetType = typeof(Texture2D);
                        item.Name = subData.name;
                        item.FullPath = filePath;
                    }
                }
            }
            return assetItem;
        }

        public override void CreateInstance(Guid guid, object loadedObj, AssetMeta meta)
        {
            if (loadedObj is not GltfImportData gltfImportData || gltfImportData.gltfImport == null) return;
            GltfImport gltfImport = gltfImportData.gltfImport;
            GltfMeta gltfMeta = meta as GltfMeta;
            if (gltfMeta == null) return;

            InstanceContainer parent = null;
            List<InstanceContainer> childs = new();

            if (gltfImport.SceneCount > 0)
            {
                InstanceContainer instance = AssetDataBaseManager.CreateInstanceContainer(guid);
                instance.Obj = gltfImportData;
                parent = instance;
            }

            if (gltfMeta.materialInfos != null)
            {
                for (int i = 0; i < gltfImport.MaterialCount; i++)
                {
                    if (i >= gltfMeta.materialInfos.Count) break;
                    Guid subGuid = gltfMeta.materialInfos[i].m_Guid;
                    if (subGuid == Guid.Empty) continue;
                    InstanceContainer instance = AssetDataBaseManager.CreateInstanceContainer(subGuid);
                    instance.Obj = gltfImport.GetMaterial(i);
                    childs.Add(instance);
                }
            }

            if (gltfMeta.textureInfos != null)
            {
                HashSet<int> overlap = new();
                for (int i = 0; i < gltfImport.TextureCount; i++)
                {
                    GLTFast.Schema.TextureBase texture = gltfImport.GetSourceTexture(i);
                    int imageIndex = texture.GetImageIndex();
                    if (overlap.Contains(imageIndex)) continue;
                    overlap.Add(imageIndex);

                    if (imageIndex >= 0 && imageIndex < gltfMeta.textureInfos.Count)
                    {
                        Guid subGuid = gltfMeta.textureInfos[imageIndex].m_Guid;
                        if (subGuid == Guid.Empty) continue;
                        InstanceContainer instance = AssetDataBaseManager.CreateInstanceContainer(subGuid);
                        instance.Obj = gltfImport.GetTexture(i);
                        childs.Add(instance);
                    }
                }
            }

            if (parent != null)
            {
                foreach (InstanceContainer child in childs)
                {
                    child.Parent = parent;
                }
                parent.Childs = childs.ToArray();
            }
        }

        public static void DeserializeSubAssetGuids(GltfMeta gltfMeta)
        {
            if (gltfMeta == null) return;
            if (gltfMeta.bufferInfos != null)
            {
                foreach (SubAssetInfo subAsset in gltfMeta.bufferInfos)
                {
                    if (Guid.TryParse(subAsset.guid, out Guid guid)) subAsset.m_Guid = guid;
                }
            }
            if (gltfMeta.materialInfos != null)
            {
                foreach (SubAssetInfo subAsset in gltfMeta.materialInfos)
                {
                    if (Guid.TryParse(subAsset.guid, out Guid guid)) subAsset.m_Guid = guid;
                }
            }
            if (gltfMeta.textureInfos != null)
            {
                foreach (SubAssetInfo subAsset in gltfMeta.textureInfos)
                {
                    if (Guid.TryParse(subAsset.guid, out Guid guid)) subAsset.m_Guid = guid;
                }
            }
            if (gltfMeta.meshInfos != null)
            {
                foreach (SubAssetInfo subAsset in gltfMeta.meshInfos)
                {
                    if (Guid.TryParse(subAsset.guid, out Guid guid)) subAsset.m_Guid = guid;
                }
            }
            if (gltfMeta.animationInfos != null)
            {
                foreach (SubAssetInfo subAsset in gltfMeta.animationInfos)
                {
                    if (Guid.TryParse(subAsset.guid, out Guid guid)) subAsset.m_Guid = guid;
                }
            }
        }
    }
}
