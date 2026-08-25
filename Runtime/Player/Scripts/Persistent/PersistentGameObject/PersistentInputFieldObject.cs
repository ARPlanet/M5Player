using System;
using Newtonsoft.Json;

namespace Module5.Player
{
    [Serializable]
    public class PersistentInputFieldObject : PersistentGameObject
    {
        public override string Type => GameObjectTypes.InputField;

        [JsonProperty("inputField")]
        public PersistentInputField inputField;
        [JsonProperty("image")]
        public PersistentRawImage rawImage;
    }
}