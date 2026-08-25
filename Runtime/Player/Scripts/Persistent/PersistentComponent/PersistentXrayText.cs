using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentXrayText : PersistentBehaviour
    {
        public XrayType xrayType;
        public int stencilId;
    }
}