using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    public class PersistentToGraphicConverter<T> : PersistentToComponenetConverter<T> where T : Graphic
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            if (comp == null) return Task.CompletedTask;
            PersistentGraphic persistentGraphic = (PersistentGraphic)persistent;

            comp.raycastTarget = persistentGraphic.raycastTarget;
            comp.raycastPadding = persistentGraphic.raycastPadding;

            return Task.CompletedTask;
        }

    }
}
