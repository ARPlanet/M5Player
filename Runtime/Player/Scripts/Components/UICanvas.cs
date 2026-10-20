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

    [Alias("Canvas")]
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

        public bool IsRootCanvas => Canvas != null && Canvas.isRootCanvas;

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

        [SerializeField] protected CanvasType canvasType = CanvasType.World; // 預設World, Prefab的Canvas預設也是World Space
        public virtual CanvasType CanvasType 
        {
            get
            {
                if (!IsRootCanvas && Canvas != null && Canvas.rootCanvas != null)
                {
                    var rootUICanvas = Canvas.rootCanvas.GetComponent<UICanvas>();
                    if (rootUICanvas != null && rootUICanvas != this)
                    {
                        return rootUICanvas.CanvasType;
                    }
                }
                return canvasType;
            }
            set
            {
                if (canvasType == value) return;
                canvasType = value;
                if (IsRootCanvas)
                {
                    if (canvasType == CanvasType.Screen)
                    {
                        ApplyScalerSettings();
                        Canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    }
                    else
                    {
                        Canvas.renderMode = RenderMode.WorldSpace;
                    }
                    SetScaleFactor();
                }
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
            if (IsRootCanvas && canvasType == CanvasType.Screen)
            {
                ApplyScalerSettings();
                SetScaleFactor();
            }
        }

        protected virtual void OnRectTransformDimensionsChange()
        {
            if (IsRootCanvas && canvasType == CanvasType.Screen)
            {
                SetScaleFactor();
            }
        }

        protected virtual void ApplyScalerSettings()
        {
            if (CanvasScaler != null && canvasType == CanvasType.Screen)
            {
                CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                CanvasScaler.referenceResolution = new Vector2(1080f, 1080f);
                CanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            }
        }

        protected virtual void SetScaleFactor()
        {
            if (canvasType == CanvasType.Screen)
            {
                float minDimension = Mathf.Min(Screen.width, Screen.height);
                CanvasScaler.dynamicPixelsPerUnit = minDimension / 1080f;
            }
            else
            {
                CanvasScaler.dynamicPixelsPerUnit = 1f;
            }
        }
    }
}
