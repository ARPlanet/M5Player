using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Module5.Player
{
    public class PersistentToVideoPlayer2DConverter : PersistentToComponenetConverter<VideoPlayer2D>
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            if (comp == null) return Task.CompletedTask;
            PersistentVideoPlayer2D persistentVideoPlayer = (PersistentVideoPlayer2D)persistent;

            comp.Url = persistentVideoPlayer.url;
            comp.IsLoop = persistentVideoPlayer.isLoop;
            comp.playOnAwake = persistentVideoPlayer.playOnAwake;
            comp.ShowPlayButton = persistentVideoPlayer.showPlayButton;
            return Task.CompletedTask;
        }

    }
}
