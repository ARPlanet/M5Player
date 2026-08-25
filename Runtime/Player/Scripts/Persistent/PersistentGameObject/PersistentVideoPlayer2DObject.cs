using System;
using Newtonsoft.Json;

namespace Module5.Player
{
    [Serializable]
    public class PersistentVideoPlayer2DObject : PersistentGameObject
    {
        public override string Type => GameObjectTypes.VideoPlayer2D;

        [JsonProperty("videoPlayer2D")]
        public PersistentVideoPlayer2D videoPlayer2D;
    }
}