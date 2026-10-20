using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    public class PersistentToAudioPlayerConverter : PersistentToComponenetConverter<AudioPlayer>
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            if (comp == null) return Task.CompletedTask;

            PersistentAudioPlayer persistentAudioPlayer = (PersistentAudioPlayer)persistent;
            comp.PlayOnAwake = persistentAudioPlayer.playOnAwake;
            comp.IsLoop = persistentAudioPlayer.loop;
            comp.PanStereo = persistentAudioPlayer.panStereo;
            comp.SpatialBlend = persistentAudioPlayer.spatialBlend;
            comp.Volume = persistentAudioPlayer.volume;
            comp.DopplerLevel = persistentAudioPlayer.dopplerLevel;
            comp.Spread = persistentAudioPlayer.spread;
            comp.RolloffMode = persistentAudioPlayer.rolloffMode;
            comp.MinDistance = persistentAudioPlayer.minDistance;
            comp.MaxDistance = persistentAudioPlayer.maxDistance;

            comp.Clip = dataBase.GetObject<AudioClip>(persistentAudioPlayer.clip);

            return Task.CompletedTask;
        }
    }
}
