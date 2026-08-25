using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    public class PersistentToInputFieldObjectConverter : PersistentToObjectConverter<PersistentInputFieldObject>
    {
        public PersistentToInputFieldObjectConverter(ICreateObjectManager createObjectManager) : base(createObjectManager) { }

        PersistentToInputFieldConverter inputFieldConverter;
        PersistentToRawImageConverter rawImageConverter;

        public override void CreateObject()
        {
            gameObject = CreateObjectManager.Instantiate(GameObjectTypes.InputField);
        }

        public override async Task RegistObject(PersistentToObjectDataBase dataBase)
        {
            await base.RegistObject(dataBase);
            if (persistent.inputField != null)
            {
                inputFieldConverter = new()
                {
                    persistent = persistent.inputField,
                    comp = gameObject.GetComponent<InputField>()
                };
                await inputFieldConverter.RegistObject(dataBase);
            }

            if (persistent.rawImage != null)
            {
                rawImageConverter = new()
                {
                    persistent = persistent.rawImage,
                    comp = gameObject.GetComponent<RawImage>()
                };
                await rawImageConverter.RegistObject(dataBase);
            }
        }
        public override async Task PersistentToObject(PersistentToObjectDataBase dataBase)
        {
            await base.PersistentToObject(dataBase);

            if (inputFieldConverter != null) await inputFieldConverter.PersistentToObjectAsync(dataBase);
            if (rawImageConverter != null) await rawImageConverter.PersistentToObjectAsync(dataBase);
        }
    }
}
