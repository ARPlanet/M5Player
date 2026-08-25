using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentClick : PersistentComponent
    {
        public bool enableLongTap;
        public float longTapThreshold;
    }
}