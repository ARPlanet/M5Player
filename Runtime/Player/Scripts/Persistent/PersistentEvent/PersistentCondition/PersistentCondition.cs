using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentCondition : Persistent
    {
        [JsonProperty("targetType")]
        public string TargetType;
        [JsonProperty("targetId")]
        public string TargetId;
        [JsonProperty("targetProperty")]
        public string TargetProperty;
        [JsonProperty("operator")]
        public string Operator;
        [JsonProperty("expectedValue")]
        public object ExpectedValue;
    }
}
