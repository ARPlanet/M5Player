using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    public class PersistentToAudioObjectConverter : PersistentToObjectConverter<PersistentAudioObject>
    {
        PersistentToAudioSourceConverter audioConverter;
        public PersistentToAudioObjectConverter(ICreateObjectManager createObjectManager) : base(createObjectManager) { }
        public override void CreateObject()
        {
            gameObject = CreateObjectManager.Instantiate(GameObjectTypes.AudioSource);
        }

        public override async Task RegistObject(PersistentToObjectDataBase dataBase)
        {
            await base.RegistObject(dataBase);
            if (persistent.audioSource != null)
            {
                audioConverter = new()
                {
                    persistent = persistent.audioSource,
                    comp = gameObject.GetComponent<AudioSource>()
                };
                await audioConverter.RegistObject(dataBase);
            }
        }
        public override async Task PersistentToObject(PersistentToObjectDataBase dataBase)
        {
            await base.PersistentToObject(dataBase);

            if (audioConverter != null) await audioConverter.PersistentToObjectAsync(dataBase);
        }
    }
}
