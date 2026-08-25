using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    public class PersistentToUICanvasConverter : PersistentToComponenetConverter<UICanvas>
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            if (comp == null) return Task.CompletedTask;
            PersistentUICanvas persistentUICanvas = (PersistentUICanvas)persistent;
            comp.CanvasType = persistentUICanvas.canvasType;
            comp.SortingOrder = persistentUICanvas.sortingOrder;
            comp.Distance = persistentUICanvas.distance;

            return Task.CompletedTask;
        }

    }
}
