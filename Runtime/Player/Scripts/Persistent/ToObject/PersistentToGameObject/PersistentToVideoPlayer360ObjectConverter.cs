using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    public class PersistentToVideoPlayer360ObjectConverter : PersistentToObjectConverter<PersistentVideoPlayer360Object>
    {
        PersistentToVideoPlayerController360Converter videoPlayer360Converter;
        public PersistentToVideoPlayer360ObjectConverter(ICreateObjectManager createObjectManager) : base(createObjectManager) { }

        public override void CreateObject()
        {
            gameObject = CreateObjectManager.Instantiate(GameObjectTypes.VideoPlayer360);
        }

        public override async Task RegistObject(PersistentToObjectDataBase dataBase)
        {
            await base.RegistObject(dataBase);
            if (persistent.videoPlayer360 != null)
            {
                videoPlayer360Converter = new()
                {
                    persistent = persistent.videoPlayer360,
                    comp = gameObject.GetComponent<VideoPlayer360>()
                };
                await videoPlayer360Converter.RegistObject(dataBase);
            }
        }
        public override async Task PersistentToObject(PersistentToObjectDataBase dataBase)
        {
            await base.PersistentToObject(dataBase);

            if (videoPlayer360Converter != null) await videoPlayer360Converter.PersistentToObjectAsync(dataBase);
        }
    }
}
