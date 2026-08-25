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
            M5Event openSceneEvent = SceneEvents.CreateStartEvent(this);
            EventManager?.TriggerEvent(openSceneEvent);
        }

        protected virtual void OnDisable()
        {
            M5Event exitSceneEvent = SceneEvents.CreateExitEvent(this);
            EventManager?.TriggerEvent(exitSceneEvent);
        }
    }
}
