using System;
using Newtonsoft.Json;

namespace Module5.Player
{
    [Serializable]
    public class PersistentAudioObject : PersistentGameObject
    {
        public override string Type => GameObjectTypes.AudioSource;

        [JsonProperty("audioSource")]
        public PersistentAudioSource audioSource;
    }
}