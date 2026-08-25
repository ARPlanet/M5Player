using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    public class PersistentToCanvasObjectConverter : PersistentToObjectConverter<PersistentCanvasObject>
    {
        public PersistentToCanvasObjectConverter(ICreateObjectManager createObjectManager) : base(createObjectManager) { }

        PersistentToUICanvasConverter uiCanvasConverter;

        public override void CreateObject()
        {
            gameObject = CreateObjectManager.Instantiate(GameObjectTypes.Canvas);
        }

        public override async Task RegistObject(PersistentToObjectDataBase dataBase)
        {
            await base.RegistObject(dataBase);
            if (persistent.uiCanvas != null)
            {
                uiCanvasConverter = new()
                {
                    persistent = persistent.uiCanvas,
                    comp = gameObject.GetComponent<UICanvas>()
                };
                await uiCanvasConverter.RegistObject(dataBase);
            }
        }
        public override async Task PersistentToObject(PersistentToObjectDataBase dataBase)
        {
            await base.PersistentToObject(dataBase);

            if (uiCanvasConverter != null) await uiCanvasConverter.PersistentToObjectAsync(dataBase);
        }
    }
}
