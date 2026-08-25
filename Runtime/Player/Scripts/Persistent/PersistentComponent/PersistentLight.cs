using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentLight : PersistentComponent
    {
        public LightType type;
        public float range;
        public float spotAngle;
        public Color color;
        public float intensity;
        public LightShadows shadows;
        public float shadowStrength;
    }
}