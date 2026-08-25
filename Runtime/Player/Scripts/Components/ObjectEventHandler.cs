using Module5.Player;
using System.Collections.Generic;
using UnityEngine;
using Module5.DI;
namespace Module5
{
    public class ObjectEventHandler : MonoBehaviour
    {
        //[Inject] public IAssetDataBaseManager AssetDataBaseManager { get; set; }
        [Inject] public IEventManager EventManager { get; set; }

        protected virtual void OnEnable()
        {
            M5Event enableEvent = ObjectEvents.CreateEnableEvent(gameObject);
            EventManager.TriggerEvent(enableEvent);
        }

        protected virtual void OnDisable()
        {
            if (!gameObject.activeInHierarchy) return;

            M5Event enableEvent = ObjectEvents.CreateDisableEvent(gameObject);
            EventManager.TriggerEvent(enableEvent);
        }
    }
}
