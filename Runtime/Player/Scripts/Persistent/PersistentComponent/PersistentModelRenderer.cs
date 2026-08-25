using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentModelRenderer : PersistentComponent
    {
        public string modelData;
        public bool playAutomatic;
    }
}