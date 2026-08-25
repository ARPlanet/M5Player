using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    public class PersistentToRawImageObjectConverter : PersistentToObjectConverter<PersistentRawImageObject>
    {
        public PersistentToRawImageObjectConverter(ICreateObjectManager createObjectManager) : base(createObjectManager) { }

        PersistentToRawImageConverter rawImageConverter;
        PersistentToRawImageSequenceConverter sequenceConverter;
        PersistentToXrayImageConverter xrayImageConverter;

        public override void CreateObject()
        {
            gameObject = CreateObjectManager.Instantiate(GameObjectTypes.RawImage);
        }

        public override async Task RegistObject(PersistentToObjectDataBase dataBase)
        {
            await base.RegistObject(dataBase);
            if (persistent.rawImage != null)
            {
                rawImageConverter = new()
                {
                    persistent = persistent.rawImage,
                    comp = gameObject.GetComponent<RawImage>()
                };
                await rawImageConverter.RegistObject(dataBase);
            }

            if (persistent.sequence != null && persistent.sequence.Enable)
            {
                sequenceConverter = new()
                {
                    persistent = persistent.sequence,
                    comp = gameObject.AddComponent<RawImageSequence>()
                };
                await sequenceConverter.RegistObject(dataBase);
            }

            if (persistent.xrayImage != null && persistent.xrayImage.Enable)
            {
                xrayImageConverter = new()
                {
                    persistent = persistent.xrayImage,
                    comp = gameObject.AddComponent<XrayImage>()
                };
                await xrayImageConverter.RegistObject(dataBase);
            }
        }
        public override async Task PersistentToObject(PersistentToObjectDataBase dataBase)
        {
            await base.PersistentToObject(dataBase);

            if (rawImageConverter != null) await rawImageConverter?.PersistentToObjectAsync(dataBase);
            if (sequenceConverter != null) await sequenceConverter.PersistentToObjectAsync(dataBase);
            if (xrayImageConverter != null) await xrayImageConverter?.PersistentToObjectAsync(dataBase);
        }
    }
}
