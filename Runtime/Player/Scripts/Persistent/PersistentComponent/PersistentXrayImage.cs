using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentXrayImage : PersistentBehaviour
    {
        public XrayType xrayType;
        public int stencilId;
        //public StencilComparison comparison;
    }
}