using System;
using Newtonsoft.Json;

namespace Module5.Player
{
    [Serializable]
    public class PersistentBoxColliderObject : PersistentGameObject
    {
        public override string Type => GameObjectTypes.BoxCollider;

        [JsonProperty("boxCollider")]
        public PersistentBoxCollider boxCollider;
        [JsonProperty("click")]
        public PersistentClick click;
    }
}