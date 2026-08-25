using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    public class PersistentToRawImageConverter : PersistentToGraphicConverter<RawImage>
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            if (comp == null) return Task.CompletedTask;
            
            base.PersistentToObjectAsync(dataBase);

            PersistentRawImage persistentRawImage = (PersistentRawImage)persistent;

            if (Guid.TryParse(persistentRawImage.texture, out Guid textureGuid))
            {
                if (dataBase.TryGetInstance(textureGuid, out InstanceContainer instance) && instance.Obj is Texture texture)
                {
                    comp.texture = texture;
                }
            }

            comp.color = persistentRawImage.color;
            //comp.maskable = persistentRawImage.maskable;

            return Task.CompletedTask;
        }

    }
}
