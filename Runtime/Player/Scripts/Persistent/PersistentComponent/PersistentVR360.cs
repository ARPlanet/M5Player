using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentVR360 : PersistentComponent
    {
        public string texture;
        public float size;
        public bool isOverrideFov;
        public float fov;
    }
}