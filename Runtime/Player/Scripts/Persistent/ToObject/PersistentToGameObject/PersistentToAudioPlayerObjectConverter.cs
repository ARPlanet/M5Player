using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    public class PersistentToAudioPlayerObjectConverter : PersistentToObjectConverter<PersistentAudioPlayerObject>
    {
        PersistentToAudioPlayerConverter audioPlayerConverter;

        public PersistentToAudioPlayerObjectConverter(ICreateObjectManager createObjectManager) : base(createObjectManager) { }

        public override void CreateObject()
        {
            gameObject = CreateObjectManager.Instantiate(GameObjectTypes.AudioPlayer);
        }

        public override async Task RegistObject(PersistentToObjectDataBase dataBase)
        {
            await base.RegistObject(dataBase);
            if (persistent.audioPlayer != null)
            {
                audioPlayerConverter = new()
                {
                    persistent = persistent.audioPlayer,
                    comp = gameObject.GetComponent<AudioPlayer>()
                };
                await audioPlayerConverter.RegistObject(dataBase);
            }
        }

        public override async Task PersistentToObject(PersistentToObjectDataBase dataBase)
        {
            await base.PersistentToObject(dataBase);

            if (audioPlayerConverter != null) await audioPlayerConverter.PersistentToObjectAsync(dataBase);
        }
    }
}
