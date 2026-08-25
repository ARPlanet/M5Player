using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Module5.Player
{
    public class PersistentToVideoPlayerController360Converter : PersistentToComponenetConverter<VideoPlayer360>
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            if (comp == null) return Task.CompletedTask;
            PersistentVideoPlayer360 persistentVideoPlayer = (PersistentVideoPlayer360)persistent;

            comp.Url = persistentVideoPlayer.url;
            comp.Size = persistentVideoPlayer.size;
            comp.IsLoop = persistentVideoPlayer.isLoop;
            comp.playOnAwake = persistentVideoPlayer.playOnAwake;
            comp.ShowPlayButton = persistentVideoPlayer.showPlayButton;
            comp.IsOverrideFov = persistentVideoPlayer.isOverrideFov;
            comp.Fov = persistentVideoPlayer.fov;
            return Task.CompletedTask;
        }

    }
}
