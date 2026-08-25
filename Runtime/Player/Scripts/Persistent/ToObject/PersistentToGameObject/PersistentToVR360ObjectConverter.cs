using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    public class PersistentToVR360ObjectConverter : PersistentToObjectConverter<PersistentVR360Object>
    {
        public PersistentToVR360ObjectConverter(ICreateObjectManager createObjectManager) : base(createObjectManager) { }


        PersistentToVR360Converter persistentToVR360Converter;

        public override void CreateObject()
        {
            gameObject = CreateObjectManager.Instantiate(GameObjectTypes.VR360);
        }

        public override async Task RegistObject(PersistentToObjectDataBase dataBase)
        {
            await base.RegistObject(dataBase);
            if (persistent.vr360 != null)
            {
                persistentToVR360Converter = new()
                {
                    persistent = persistent.vr360,
                    comp = gameObject.GetComponent<VR360>()
                };
                await persistentToVR360Converter.RegistObject(dataBase);
            }
        }
        public override async Task PersistentToObject(PersistentToObjectDataBase dataBase)
        {
            await base.PersistentToObject(dataBase);

            if (persistentToVR360Converter != null) await persistentToVR360Converter.PersistentToObjectAsync(dataBase);
        }
    }
}
