using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    [System.Serializable]
    public class AudioMeta : AssetMeta
    {
        //public AudioImportSettings settings;
    }

    public class AudioStorage : AssetStorage<AudioMeta>
    {
        public static readonly string[] Extensions = { ".mp3", ".wav", ".wave", ".ogg" };
        public override IReadOnlyList<string> SupportedExtensions => Extensions;
        public override Type RuntimeAssetType => typeof(AudioClip);

        protected readonly AudioImporter audioImporter = new();

        public AudioStorage(IAssetDataBaseManager assetDataBaseManager) : base(assetDataBaseManager) { }

        public override async Task<object> LoadAssetObjectAsync(string filePath, AssetMeta meta)
        {
            AudioClip audioClip = await audioImporter.ImportAsync(new Uri(filePath, UriKind.Absolute));
            return audioClip;
        }
    }
}
