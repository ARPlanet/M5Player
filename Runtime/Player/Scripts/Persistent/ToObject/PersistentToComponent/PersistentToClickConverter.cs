using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    public class PersistentToClickConverter : PersistentToComponenetConverter<Click>
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            if (comp == null) return Task.CompletedTask;
            PersistentClick persistentPressHandler = (PersistentClick)persistent;
            comp.enableLongTap = persistentPressHandler.enableLongTap;
            comp.longTapThreshold = persistentPressHandler.longTapThreshold;

            return Task.CompletedTask;
        }
    }
}
