using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    public class PersistentToPanelObjectConverter : PersistentToObjectConverter<PersistentPanelObject>
    {
        public PersistentToPanelObjectConverter(ICreateObjectManager createObjectManager) : base(createObjectManager) { }
        public override void CreateObject()
        {
            gameObject = CreateObjectManager.Instantiate(GameObjectTypes.Panel);
        }

        public override Task RegistObject(PersistentToObjectDataBase dataBase)
        {
            return base.RegistObject(dataBase);
        }
        public override Task PersistentToObject(PersistentToObjectDataBase dataBase)
        {
            return base.PersistentToObject(dataBase);
        }
    }
}
