using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Module5.Player;
using UnityEngine;
using UnityEngine.Rendering;
using Module5.DI;
namespace Module5.Player
{
    public class SceneRoot : MonoBehaviour, ISceneRoot
    {
        [Inject] public IEventManager EventManager {  get; set; }

        public SceneData sceneData;
        public SceneData SceneData
        {
            get => sceneData;
            set => sceneData = value;
        }

        protected virtual void OnEnable()
        {
            // Note: StartEvent is triggered actively by EnterSceneAsync after gameObject.SetActive(true)
            // to ensure all EventComponents in the hierarchy have completed their OnEnable registration.
        }

        protected virtual void OnDisable()
        {
            // Note: ExitEvent is triggered actively by SceneManager prior to deactivation/destruction
            // to allow async exit actions (e.g. fade out) to complete cleanly before rules are cleared.
        }

        public virtual Task EnterSceneAsync()
        {
            if (EventManager != null)
            {
                // 1. 開啟事件紀錄 (暫存激活期間組件自啟動所產生的事件)
                EventManager.BeginRecordEvents();

                // 2. 激活物件樹：整棵樹所有組件執行 Awake 與 OnEnable
                //    確保所有 EventComponent 完成規則註冊 (RegisterRules)
                gameObject.SetActive(true);

                // 3. 所有規則皆已就緒後，正式觸發場景開場事件
                M5Event openSceneEvent = new SceneEvents.StartEvent(this);
                EventManager.TriggerEvent(openSceneEvent);

                // 4. 派發並執行所有累積的事件 (自啟動事件與 StartEvent)
                EventManager.TriggerRecordedEvents();
            }
            else
            {
                gameObject.SetActive(true);
            }

            return Task.CompletedTask;
        }

        public virtual async Task ExitSceneAsync(float timeoutSeconds = 3f)
        {
            if (EventManager != null)
            {
                M5Event exitSceneEvent = new SceneEvents.ExitEvent(this);
                await EventManager.TriggerEventAsync(exitSceneEvent, timeoutSeconds);
            }
        }
    }
}
