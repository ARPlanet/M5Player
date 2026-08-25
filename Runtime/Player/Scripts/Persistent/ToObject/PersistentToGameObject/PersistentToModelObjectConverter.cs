using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    public class PersistentToModelObjectConverter : PersistentToObjectConverter<PersistentModelObject>
    {
        public PersistentToModelObjectConverter(ICreateObjectManager createObjectManager) : base(createObjectManager) { }

        PersistentToModelRendererConverter modelRenderConverter;

        public override void CreateObject()
        {
            gameObject = CreateObjectManager.Instantiate(GameObjectTypes.ModelRender);
        }

        public override async Task RegistObject(PersistentToObjectDataBase dataBase)
        {
            await base.RegistObject(dataBase);
            if (persistent.modelRenderer != null)
            {
                modelRenderConverter = new()
                {
                    persistent = persistent.modelRenderer,
                    comp = gameObject.GetComponent<ModelRenderer>()
                };
                await modelRenderConverter.RegistObject(dataBase);
            }
        }

        public override async Task PersistentToObject(PersistentToObjectDataBase dataBase)
        {
            await base.PersistentToObject(dataBase);

            if (modelRenderConverter != null) await modelRenderConverter.PersistentToObjectAsync(dataBase);
        }
    }
}
