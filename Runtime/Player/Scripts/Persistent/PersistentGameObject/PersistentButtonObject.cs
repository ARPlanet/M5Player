using System;
using Newtonsoft.Json;

namespace Module5.Player
{
    [Serializable]
    public class PersistentButtonObject : PersistentGameObject
    {
        public override string Type => GameObjectTypes.Button;

        [JsonProperty("image")]
        public PersistentRawImage rawImage;
        //[JsonProperty("button")]
        //public PersistentButton button;
        [JsonProperty("click")]
        public PersistentClick click;
    }
}