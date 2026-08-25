using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentUICanvas : PersistentComponent
    {
        public CanvasType canvasType;
        public float distance;
        public int sortingOrder;
    }
}