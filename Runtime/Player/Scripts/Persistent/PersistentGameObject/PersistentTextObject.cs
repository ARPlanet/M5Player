using System;
using Newtonsoft.Json;

namespace Module5.Player
{
    [Serializable]
    public class PersistentTextObject : PersistentGameObject
    {
        public override string Type => GameObjectTypes.Text;

        [JsonProperty("text")]
        public PersistentText text;
        [JsonProperty("xrayText")]
        public PersistentXrayText xrayText;
    }
}