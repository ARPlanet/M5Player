using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Module5.Player
{
    public class Folder { }

    public class FolderStorage : AssetStorage<AssetMeta>
    {
        public override IReadOnlyList<string> SupportedExtensions => Array.Empty<string>();
        public override Type RuntimeAssetType => typeof(Folder);

        public FolderStorage(IAssetDataBaseManager assetDataBaseManager) : base(assetDataBaseManager) { }

        public override Task<object> LoadAssetObjectAsync(string filePath, AssetMeta meta)
        {
            return Task.FromResult<object>(new Folder());
        }

        public override void CreateInstance(Guid guid, object loadedObj, AssetMeta meta)
        {
        }
    }
}
