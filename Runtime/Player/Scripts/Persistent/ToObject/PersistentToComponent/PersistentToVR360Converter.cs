using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Module5.Player
{
    public class PersistentToVR360Converter : PersistentToComponenetConverter<VR360>
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            if (comp == null) return Task.CompletedTask;
            PersistentVR360 persistentVR360 = (PersistentVR360)persistent;

            if (Guid.TryParse(persistentVR360.texture, out Guid textureGuid))
            {
                if (dataBase.TryGetInstance(textureGuid, out InstanceContainer instance) && instance.Obj is Texture texture)
                {
                    comp.Texture = texture;
                }
            }

            comp.Size = persistentVR360.size;
            comp.IsOverrideFov = persistentVR360.isOverrideFov;
            comp.Fov = persistentVR360.fov;
            return Task.CompletedTask;
        }

    }
}
