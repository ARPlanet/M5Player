using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [System.Serializable]
    public class AnchorQrData : Anchor
    {
        public new const string Type = "Qr";

        public override string AnchorTyoe => Type;
        public virtual string Content { get; set; }
    }
}
