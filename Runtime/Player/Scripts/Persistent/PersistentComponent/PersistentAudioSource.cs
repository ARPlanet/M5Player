using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentAudioSource : PersistentComponent
    {
        public string clip;

        public bool playOnAwake;
        public bool loop;
        public float panStereo; // Stereo Pan
        public float spatialBlend;
        public float volume;
        public float dopplerLevel;
        public float spread;
        public AudioRolloffMode rolloffMode; // Volume Rolloff
        public float minDistance;
        public float maxDistance;
    }
}