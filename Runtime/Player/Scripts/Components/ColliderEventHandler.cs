using Module5.Player;
using System.Collections.Generic;
using UnityEngine;
using Module5.DI;
namespace Module5
{
    public class ColliderEventHandler : MonoBehaviour
    {
        //[Inject] public IAssetDataBaseManager AssetDataBaseManager { get; set; }
        [Inject] public IEventManager EventManager { get; set; }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!gameObject.activeInHierarchy) return;

            Collider col = GetComponent<Collider>();

            M5Event enableEvent = TriggerEvents.CreateEnterEvent(col, col, other);
            EventManager.TriggerEvent(enableEvent);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (!gameObject.activeInHierarchy) return;

            Collider col = GetComponent<Collider>();

            M5Event enableEvent = TriggerEvents.CreateExitEvent(col, col, other);
            EventManager.TriggerEvent(enableEvent);
        }
    }
}
