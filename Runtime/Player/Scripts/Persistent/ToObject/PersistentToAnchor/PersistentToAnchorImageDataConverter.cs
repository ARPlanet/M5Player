using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    public class PersistentToAnchorImageDataConverter : PersistentToAnchorDataConverter
    {
        public PersistentToAnchorImageDataConverter(IAssetLoaderManager assetLoaderManager) : base(assetLoaderManager) { }

        public override async Task<Anchor> ConvertAsync(PersistentAnchor persistent)
        {
            var persistentImage = (PersistentAnchorImage)persistent;
            var imageData = await ConvertInternalAsync<AnchorImageData>(persistent);

            if (Guid.TryParse(persistentImage.textureId, out Guid guid))
            {
                // 使用注入的資料庫載入實例
                InstanceContainer instance = await _assetLoaderManager.LoadInstance(guid);
                if (instance != null)
                {
                    imageData.Image = instance.Obj as Texture;
                }
            }
            imageData.Size = persistentImage.size;

            return imageData;
        }
    }
}