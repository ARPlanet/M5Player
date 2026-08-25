using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Module5.DI;
namespace Module5.Player
{

    public enum CanvasType
    {
        Screen,
        World,
    }


    public class UICanvas : MonoBehaviour
    {
        [Inject] ISceneManager SceneManager {  get; set; }
        protected Canvas canvas;

        public Canvas Canvas 
        {
            get
            {
                if(canvas == null)
                {
                    canvas = GetComponent<Canvas>();
                }
                return canvas;
            }
        }

        public bool IsRootCanvas => Canvas.isRootCanvas;

        public virtual int SortingOrder
        {
            get => Canvas.sortingOrder;
            set 
            { 
                Canvas.sortingOrder = value; 
            }
        }

        public virtual float Distance
        {
            get => Canvas.planeDistance;
            set
            {
                Canvas.planeDistance = value;
            }
        }

        [SerializeField] protected CanvasType canvasType;
        public virtual CanvasType CanvasType 
        {
            get => canvasType;
            set
            {
                if (canvasType == value) return;
                canvasType = value;
                if (canvasType == CanvasType.Screen)
                {
                    Canvas.renderMode = RenderMode.ScreenSpaceCamera;
                }
                else
                {
                    Canvas.renderMode = RenderMode.WorldSpace;
                }
                SetScaleFactor();
            }
        }

        protected CanvasScaler canvasScaler;
        public CanvasScaler CanvasScaler
        {
            get
            {
                if (canvasScaler == null)
                {
                    canvasScaler = GetComponent<CanvasScaler>();
                }
                return canvasScaler;
            }
        }

        protected virtual void Awake()
        {
            if (Canvas.worldCamera == null)
            {
                Canvas.worldCamera = SceneManager.VirtualCamera;
            }
            if (!IsRootCanvas)
            {
                Canvas.overrideSorting = true;
            }
        }

        protected virtual void SetScaleFactor()
        {
            if (canvasType == CanvasType.Screen)
            {
                //CanvasScaler.dynamicPixelsPerUnit = Screen.height / 1920;
            }
            else
            {
                CanvasScaler.dynamicPixelsPerUnit = 1f;
            }
        }
    }
}