using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    public class PersistentToBoxColliderObjectConverter : PersistentToObjectConverter<PersistentBoxColliderObject>
    {
        public PersistentToBoxColliderObjectConverter(ICreateObjectManager createObjectManager) : base(createObjectManager) { }

        //PersistentToButtonConverter buttonConverter;
        PersistentToBoxColliderConverter boxColliderConverter;
        PersistentToClickConverter clickConverter;

        public override void CreateObject()
        {
            gameObject = CreateObjectManager.Instantiate(GameObjectTypes.BoxCollider);
        }

        public override async Task RegistObject(PersistentToObjectDataBase dataBase)
        {
            await base.RegistObject(dataBase);
            //if (persistent.button != null)
            //{
            //    buttonConverter = new()
            //    {
            //        persistent = persistent.button,
            //        comp = gameObject.GetComponent<Button>()
            //    };
            //    await buttonConverter.RegistObject(dataBase);
            //}

            if (persistent.boxCollider != null)
            {
                boxColliderConverter = new()
                {
                    persistent = persistent.boxCollider,
                    comp = gameObject.GetComponent<BoxCollider>()
                };
                await boxColliderConverter.RegistObject(dataBase);
            }

            if (persistent.click != null)
            {
                clickConverter = new()
                {
                    persistent = persistent.click,
                    comp = gameObject.GetComponent<Click>()
                };
                await clickConverter.RegistObject(dataBase);
            }
        }
        public override async Task PersistentToObject(PersistentToObjectDataBase dataBase)
        {
            await base.PersistentToObject(dataBase);

            //if (buttonConverter != null) await buttonConverter.PersistentToObjectAsync(dataBase);
            if (boxColliderConverter != null) await boxColliderConverter.PersistentToObjectAsync(dataBase);
            if (clickConverter != null) await clickConverter.PersistentToObjectAsync(dataBase);
        }
    }
}
