using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentVideoPlayer2D : PersistentComponent
    {
        public string url;
        public bool playOnAwake;
        public bool isLoop;
        public bool showPlayButton;
    }
}