using System;
using Newtonsoft.Json;

namespace Module5.Player
{
    [Serializable]
    public class PersistentModelObject : PersistentGameObject
    {
        public override string Type => GameObjectTypes.ModelRender;

        [JsonProperty("modelRenderer")]
        public PersistentModelRenderer modelRenderer;
    }
}