using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [System.Serializable]
    public class AnchorImageData : Anchor
    {
        public new const string Type = "Image";

        public override string AnchorTyoe => Type;
        public virtual Texture Image { get; set; }
        public virtual float Size { get; set; }
    }
}
