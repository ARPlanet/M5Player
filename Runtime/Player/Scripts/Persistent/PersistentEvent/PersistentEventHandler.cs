using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentEventHandler : PersistentObject
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("enabled")]
        public bool Enabled;
        [JsonProperty("eventType")]
        public string EventType;
        [JsonProperty("sourceFilter")]
        public string[] SourceFilter;
        [JsonProperty("priority")]
        public int Priority;
        [JsonProperty("delay")]
        public float Delay;
        [JsonProperty("conditions")]
        public PersistentCondition[] Conditions;
        [JsonProperty("commands")]
        public PersistentCommand[] Commands;
    }
}