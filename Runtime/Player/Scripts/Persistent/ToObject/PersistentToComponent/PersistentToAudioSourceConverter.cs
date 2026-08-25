using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    public class PersistentToAudioSourceConverter : PersistentToComponenetConverter<AudioSource>
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            if(comp == null)  return Task.CompletedTask;
            PersistentAudioSource persistentAudioSource = (PersistentAudioSource)persistent;
            comp.playOnAwake = persistentAudioSource.playOnAwake;
            comp.loop = persistentAudioSource.loop;
            comp.panStereo = persistentAudioSource.panStereo;
            comp.spatialBlend = persistentAudioSource.spatialBlend;
            comp.volume = persistentAudioSource.volume;
            comp.dopplerLevel = persistentAudioSource.dopplerLevel;
            comp.spread = persistentAudioSource.spread;
            comp.rolloffMode = persistentAudioSource.rolloffMode;
            comp.minDistance = persistentAudioSource.minDistance;
            comp.maxDistance = persistentAudioSource.maxDistance;

            comp.clip = dataBase.GetObject<AudioClip>(persistentAudioSource.clip);


            return Task.CompletedTask;
        }
    }
}
