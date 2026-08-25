using System;
using Newtonsoft.Json;

namespace Module5.Player
{
    [Serializable]
    public class PersistentVideoPlayer360Object : PersistentGameObject
    {
        public override string Type => GameObjectTypes.VideoPlayer360;

        [JsonProperty("videoPlayer360")]
        public PersistentVideoPlayer360 videoPlayer360;
    }
}