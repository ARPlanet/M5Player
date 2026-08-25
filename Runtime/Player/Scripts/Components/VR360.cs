using GLTFast;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Module5.DI;
namespace Module5.Player
{
    public interface IVR360
    {

    }

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

        [SerializeField] protected bool isOverrideFov = false;
        public bool IsOverrideFov
        {
            get => isOverrideFov;
            set
            {
                isOverrideFov = value;
                if (isActiveAndEnabled)
                {
                    SyncVirtualCamera syncVirtualCamera = SceneManager.VirtualCamera.GetComponent<SyncVirtualCamera>();
                    if (syncVirtualCamera != null)
                    {
                        syncVirtualCamera.enabled = !IsOverrideFov;
                    }
                    if (value)
                    {
                        SceneManager.VirtualCamera.fieldOfView = fov;
                    }
                }
            }
        }

        [SerializeField] protected float fov = 60f;
        public float Fov
        {
            get => fov;
            set
            {
                fov = value;
                if (isActiveAndEnabled && IsOverrideFov)
                {
                    SceneManager.VirtualCamera.fieldOfView = fov;
                }
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
            if (isOverrideFov)
            {
                SyncVirtualCamera syncVirtualCamera = SceneManager.VirtualCamera.GetComponent<SyncVirtualCamera>();
                if (syncVirtualCamera != null)
                {
                    syncVirtualCamera.enabled = false;
                }
                SceneManager.VirtualCamera.fieldOfView = fov;
            }
        }

        protected virtual void OnDisable()
        {
            SyncVirtualCamera syncVirtualCamera = SceneManager.VirtualCamera.GetComponent<SyncVirtualCamera>();
            if (syncVirtualCamera != null)
            {
                syncVirtualCamera.enabled = true;
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