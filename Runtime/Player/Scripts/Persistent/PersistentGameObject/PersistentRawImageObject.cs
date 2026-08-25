using System;
using Newtonsoft.Json;

namespace Module5.Player
{
    [Serializable]
    public class PersistentRawImageObject : PersistentGameObject
    {
        public override string Type => GameObjectTypes.RawImage;

        [JsonProperty("rawImage")]
        public PersistentRawImage rawImage;
        [JsonProperty("sequence")]
        public PersistentRawImageSequence sequence;
        [JsonProperty("xrayImage")]
        public PersistentXrayImage xrayImage;
    }
}