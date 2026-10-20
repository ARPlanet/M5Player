using System;
using Newtonsoft.Json;

namespace Module5.Player
{
    [Serializable]
    public class PersistentAudioPlayerObject : PersistentGameObject
    {
        public override string Type => GameObjectTypes.AudioPlayer;

        [JsonProperty("audioPlayer")]
        public PersistentAudioPlayer audioPlayer;
    }
}
