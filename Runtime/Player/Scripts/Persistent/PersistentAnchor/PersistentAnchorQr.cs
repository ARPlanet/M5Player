using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentAnchorQr : PersistentAnchor
    {
        [JsonProperty("type")]
        public override string Type => AnchorQrData.Type;
        [JsonProperty("content")]
        public string content;
    }
}