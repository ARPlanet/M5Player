using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    public class PersistentToXrayImageConverter : PersistentToBehaviourConverter<XrayImage>
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            base.PersistentToObjectAsync(dataBase);
            if (comp == null) return Task.CompletedTask;
            PersistentXrayImage persistentXrayImage = (PersistentXrayImage)persistent;

            comp.XrayType = persistentXrayImage.xrayType;
            comp.StencilId = persistentXrayImage.stencilId;
            //comp.Comparison = persistentXrayImage.comparison;

            return Task.CompletedTask;
        }

    }
}
