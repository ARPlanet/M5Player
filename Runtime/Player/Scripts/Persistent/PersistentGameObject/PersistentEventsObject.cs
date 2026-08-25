using System;
using Newtonsoft.Json;

namespace Module5.Player
{
    [Serializable]
    public class PersistentEventsObject : PersistentGameObject
    {
        public override string Type => GameObjectTypes.Event;

        [JsonProperty("eventComponent")]
        public PersistentEventComponent eventComponent;
    }
}