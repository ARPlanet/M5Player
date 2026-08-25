using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    public class PersistentToLightObjectConverter : PersistentToObjectConverter<PersistentLightObject>
    {
        public PersistentToLightObjectConverter(ICreateObjectManager createObjectManager) : base(createObjectManager) { }

        PersistentToLightConverter lightConverter;

        public override void CreateObject()
        {
            gameObject = CreateObjectManager.Instantiate(GameObjectTypes.Light);
        }

        public override async Task RegistObject(PersistentToObjectDataBase dataBase)
        {
            await base.RegistObject(dataBase);
            if (persistent.light != null)
            {
                lightConverter = new()
                {
                    persistent = persistent.light,
                    comp = gameObject.GetComponent<Light>()
                };
                await lightConverter.RegistObject(dataBase);
            }
        }
        public override async Task PersistentToObject(PersistentToObjectDataBase dataBase)
        {
            await base.PersistentToObject(dataBase);

            if (lightConverter != null) await lightConverter.PersistentToObjectAsync(dataBase);
        }
    }
}
