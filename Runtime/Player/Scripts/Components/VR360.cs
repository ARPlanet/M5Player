using GLTFast;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Module5.DI;
namespace Module5.Player
{
    public enum VR360SphereType
    {
        Background = 0,   // 全景背景：ZTest Always, ZWrite Off, Queue Background (預設)
        SpatialObject = 1 // 空間物件：ZTest LEqual, ZWrite On, Queue Geometry
    }

    public interface IVR360
    {
        VR360SphereType SphereType { get; set; }
        bool IsOverrideFov { get; set; }
        float Fov { get; set; }
        float Size { get; set; }
    }

    [Alias("VR 360")]
    public class VR360 : MonoBehaviour, IVR360
    {
        [Inject] ISceneManager SceneManager { get; set; }

        [SerializeField] protected MeshRenderer meshRenderer;

        [SerializeField] protected Material materialInstance;
        protected Material MaterialInstance
        {
            get
            {
                if (materialInstance == null)
                {
                    materialInstance = meshRenderer.material; // unity會自動複製一個新的材質球Instance
                }
                return materialInstance;
            }
        }

        [Header("360 Sphere Settings")]
        [SerializeField] protected VR360SphereType sphereType = VR360SphereType.Background;
        public VR360SphereType SphereType
        {
            get => sphereType;
            set
            {
                sphereType = value;
                ApplySphereTypeToMaterial();
            }
        }

        [SerializeField] protected bool isOverrideFov = false;
        public bool IsOverrideFov
        {
            get => isOverrideFov;
            set
            {
                isOverrideFov = value;
                ApplyFovToCamera();
            }
        }

        [SerializeField] protected float fov = 60f;
        public float Fov
        {
            get => fov;
            set
            {
                fov = value;
                if (isOverrideFov)
                {
                    ApplyFovToCamera();
                }
            }
        }

        public void ApplySphereTypeToMaterial()
        {
            if (MaterialInstance == null) return;

            if (sphereType == VR360SphereType.Background)
            {
                MaterialInstance.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
                MaterialInstance.SetInt("_ZWrite", 0);
                MaterialInstance.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Background;
            }
            else
            {
                MaterialInstance.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
                MaterialInstance.SetInt("_ZWrite", 1);
                MaterialInstance.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            }
        }

        private void ApplyFovToCamera()
        {
            if (SceneManager == null || SceneManager.VirtualCamera == null) return;
            SyncVirtualCamera syncVirtualCamera = SceneManager.VirtualCamera.GetComponent<SyncVirtualCamera>();
            if (syncVirtualCamera != null)
            {
                syncVirtualCamera.SetFovOverride(isActiveAndEnabled && isOverrideFov, fov);
            }
            else if (isActiveAndEnabled && isOverrideFov)
            {
                SceneManager.VirtualCamera.fieldOfView = fov;
            }
        }

        [SerializeField] protected Texture texture;
        [RegisterReference]
        public Texture Texture
        {
            get 
            {
                texture = MaterialInstance.mainTexture;
                return texture;
            }
            set
            {
                texture = value;
                MaterialInstance.mainTexture = texture;
            }
        }

        public float Size
        {
            get
            {
                return meshRenderer.transform.localScale.x;
            }
            set
            {
                meshRenderer.transform.localScale = new Vector3(value, value, value);
            }
        }

        public float Rotation
        {
            get
            {
                return MaterialInstance.GetFloat("_Rotation");
            }
            set
            {
                MaterialInstance.SetFloat("_Rotation", value);
            }
        }

        protected virtual void OnEnable()
        {
            ApplySphereTypeToMaterial();
            if (isOverrideFov)
            {
                ApplyFovToCamera();
            }
        }

        protected virtual void OnDisable()
        {
            if (isOverrideFov && SceneManager != null && SceneManager.VirtualCamera != null)
            {
                SyncVirtualCamera syncVirtualCamera = SceneManager.VirtualCamera.GetComponent<SyncVirtualCamera>();
                if (syncVirtualCamera != null)
                {
                    syncVirtualCamera.SetFovOverride(false);
                }
            }
        }
        private void OnDestroy()
        {
            if(materialInstance != null)
            {
                Destroy(materialInstance);
            }
        }
    }
}