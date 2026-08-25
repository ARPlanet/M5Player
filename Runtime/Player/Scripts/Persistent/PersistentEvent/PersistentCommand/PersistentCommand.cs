using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentCommand : Persistent
    {
        [JsonProperty("delay")]
        public float Delay;
        [JsonProperty("type")]
        public string Type;
        [JsonProperty("parameters")]
        public Dictionary<string, object> Parameters;
    }
}
