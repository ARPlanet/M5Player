using System;
using Newtonsoft.Json;

namespace Module5.Player
{
    [Serializable]
    public class PersistentVR360Object : PersistentGameObject
    {
        public override string Type => GameObjectTypes.VR360;

        [JsonProperty("vr360")]
        public PersistentVR360 vr360;
    }
}