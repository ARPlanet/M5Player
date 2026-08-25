using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    [Obsolete]
    public class PersistentToCameraConverter : PersistentToComponenetConverter<Camera>
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            if (comp == null) return Task.CompletedTask;
            comp.clearFlags = CameraClearFlags.SolidColor;
            comp.backgroundColor = Color.clear;
            comp.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "UI");
            comp.nearClipPlane = 0.01f;
            return Task.CompletedTask;
        }
    }
}
