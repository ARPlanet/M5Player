using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    public class PersistentToEventObjectConverter : PersistentToObjectConverter<PersistentEventsObject>
    {
        public PersistentToEventObjectConverter(ICreateObjectManager createObjectManager) : base(createObjectManager) { }

        PersistentToM5EventConverter eventConverter;

        public override void CreateObject()
        {
            gameObject = CreateObjectManager.Instantiate(GameObjectTypes.Event);
        }

        public override async Task RegistObject(PersistentToObjectDataBase dataBase)
        {
            await base.RegistObject(dataBase);
            if (persistent.eventComponent != null)
            {
                eventConverter = new()
                {
                    persistent = persistent.eventComponent,
                    comp = gameObject.GetComponent<EventComponent>()
                };
                await eventConverter.RegistObject(dataBase);
            }
        }
        public override async Task PersistentToObject(PersistentToObjectDataBase dataBase)
        {
            await base.PersistentToObject(dataBase);

            if (eventConverter != null) await eventConverter.PersistentToObjectAsync(dataBase);
        }
    }
}
