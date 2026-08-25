using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    public class PersistentToTextObjectConverter : PersistentToObjectConverter<PersistentTextObject>
    {
        public PersistentToTextObjectConverter(ICreateObjectManager createObjectManager) : base(createObjectManager) { }

        PersistentToTextConverter textConverter;
        PersistentToXrayTextConverter xrayTextConverter;

        public override void CreateObject()
        {
            gameObject = CreateObjectManager.Instantiate(GameObjectTypes.Text);
        }

        public override async Task RegistObject(PersistentToObjectDataBase dataBase)
        {
            await base.RegistObject(dataBase);
            if (persistent.text != null)
            {
                textConverter = new()
                {
                    persistent = persistent.text,
                    comp = gameObject.GetComponent<Text>()
                };
                await textConverter.RegistObject(dataBase);
            }
            if (persistent.xrayText != null && persistent.xrayText.Enable)
            {
                xrayTextConverter = new()
                {
                    persistent = persistent.xrayText,
                    comp = gameObject.AddComponent<XrayText>()
                };
                await xrayTextConverter.RegistObject(dataBase);
            }
        }
        public override async Task PersistentToObject(PersistentToObjectDataBase dataBase)
        {
            await base.PersistentToObject(dataBase);

            if (textConverter != null) await textConverter.PersistentToObjectAsync(dataBase);
            if (xrayTextConverter != null) await xrayTextConverter.PersistentToObjectAsync(dataBase);
        }
    }
}
