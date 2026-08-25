using UnityEngine;
using Module5.DI;
namespace Module5.Player
{
    public class EventComponent : MonoBehaviour
    {
        [Inject] public IEventManager EventManager { get; set; }

#if UNITY_EDITOR
        private IAssetDataBaseManager _assetDataBaseManager;
        [Inject]
        public IAssetDataBaseManager AssetDataBaseManager
        {
            get => _assetDataBaseManager;
            set
            {
                _assetDataBaseManager = value;
                InjectAssetDataBaseManager();
            }
        }

        private void Awake()
        {
            InjectAssetDataBaseManager();
        }

        public void InjectAssetDataBaseManager()
        {
            if (_assetDataBaseManager == null || eventHandlers == null) return;
            foreach (var handler in eventHandlers)
            {
                handler?.SetAssetDataBaseManager(_assetDataBaseManager);
            }
        }
#endif

        /// <summary>
        /// The list of event handler rules associated with this component.
        /// This is populated from the serialized scene data.
        /// </summary>
        public M5EventHandler[] eventHandlers;

        private void OnEnable()
        {
            if (EventManager != null && eventHandlers != null && eventHandlers.Length > 0)
            {
                EventManager.RegisterRules(eventHandlers);
            }
        }

        private void OnDisable()
        {
            if (EventManager != null && eventHandlers != null && eventHandlers.Length > 0)
            {
                EventManager.UnregisterRules(eventHandlers);
            }
        }
    }
}