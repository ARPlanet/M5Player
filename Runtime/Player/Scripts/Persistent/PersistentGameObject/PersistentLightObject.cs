using System;
using Newtonsoft.Json;

namespace Module5.Player
{
    [Serializable]
    public class PersistentLightObject : PersistentGameObject
    {
        public override string Type => GameObjectTypes.Light;

        [JsonProperty("light")]
        public PersistentLight light;
    }
}