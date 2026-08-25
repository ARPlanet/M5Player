using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GLTFast;
using UnityEngine;

namespace Module5.Player
{
    public class PersistentToModelRendererConverter : PersistentToComponenetConverter<ModelRenderer>
    {
        public override async Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            if (comp == null) return;
            PersistentModelRenderer persistentModelRender = persistent as PersistentModelRenderer;

            comp.playAutomatically = persistentModelRender.playAutomatic; // 必須先設定再設定模型

            if (Guid.TryParse(persistentModelRender.modelData, out Guid modelDataAssetGuid))
            {
                if (dataBase.TryGetInstance(modelDataAssetGuid, out InstanceContainer instance) && instance.Obj is IModelData modelData)
                {
                    await comp.SetModelData(modelData);
                }
            }

        }
    }
}
