using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    [System.Serializable]
    public class PngMeta : AssetMeta
    {
        public ImageImportSettings settings = new();
    }

    public class PngStorage : AssetStorage<PngMeta>
    {
        public static readonly string[] Extensions = { ".png", ".jpg", ".jpeg" };
        public override IReadOnlyList<string> SupportedExtensions => Extensions;
        public override Type RuntimeAssetType => typeof(Texture2D);

        protected readonly PngImporter pngImporter = new();

        public PngStorage(IAssetDataBaseManager assetDataBaseManager) : base(assetDataBaseManager) { }

        public override async Task<object> LoadAssetObjectAsync(string filePath, AssetMeta meta)
        {
            PngMeta pngMeta = meta as PngMeta ?? new PngMeta();
            Texture2D texture = await pngImporter.ImportAsync(filePath, pngMeta.settings);
            return texture;
        }
    }
}
