using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentAnchorImage : PersistentAnchor
    {
        [JsonProperty("type")]
        public override string Type => AnchorImageData.Type;
        [JsonProperty("textureId")]
        public string textureId;
        [JsonProperty("size")]
        public float size;
    }
}