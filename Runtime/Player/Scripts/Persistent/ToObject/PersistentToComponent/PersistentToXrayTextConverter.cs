using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    public class PersistentToXrayTextConverter : PersistentToBehaviourConverter<XrayText>
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            base.PersistentToObjectAsync(dataBase);

            if (comp == null) return Task.CompletedTask;
            PersistentXrayText persistentXrayText = (PersistentXrayText)persistent;

            comp.XrayType = persistentXrayText.xrayType;
            comp.StencilId = persistentXrayText.stencilId;

            return Task.CompletedTask;
        }

    }
}
