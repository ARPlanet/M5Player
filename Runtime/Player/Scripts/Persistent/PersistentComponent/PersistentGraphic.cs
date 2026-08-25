using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentGraphic : PersistentComponent
    {
        public bool raycastTarget = true;
        public Vector4 raycastPadding;
    }
}