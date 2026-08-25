using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    public class PersistentToVideoPlayer2DObjectConverter : PersistentToObjectConverter<PersistentVideoPlayer2DObject>
    {
        public PersistentToVideoPlayer2DObjectConverter(ICreateObjectManager createObjectManager) : base(createObjectManager) { }

        PersistentToVideoPlayer2DConverter videoPlayer2DConverter;

        public override void CreateObject()
        {
            gameObject = CreateObjectManager.Instantiate(GameObjectTypes.VideoPlayer2D);
        }

        public override async Task RegistObject(PersistentToObjectDataBase dataBase)
        {
            await base.RegistObject(dataBase);
            if (persistent.videoPlayer2D != null)
            {
                videoPlayer2DConverter = new()
                {
                    persistent = persistent.videoPlayer2D,
                    comp = gameObject.GetComponent<VideoPlayer2D>()
                };
                await videoPlayer2DConverter.RegistObject(dataBase);
            }
        }
        public override async Task PersistentToObject(PersistentToObjectDataBase dataBase)
        {
            await base.PersistentToObject(dataBase);

            if (videoPlayer2DConverter != null) await videoPlayer2DConverter.PersistentToObjectAsync(dataBase);
        }
    }
}
