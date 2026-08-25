using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentVideoPlayer360 : PersistentComponent
    {
        public string url;
        public float size;
        public bool playOnAwake;
        public bool isLoop;
        public bool showPlayButton;
        public bool isOverrideFov;
        public float fov;
    }
}