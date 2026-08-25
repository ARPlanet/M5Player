using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentRawImageSequence : PersistentBehaviour
    {
        public Vector2Int size;
        public int frameLength;
        public float speed;
        public SequenceMode mode;
        public bool playAuto;
    }
}