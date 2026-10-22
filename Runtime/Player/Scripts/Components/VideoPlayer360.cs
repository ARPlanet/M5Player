using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using Module5.DI;

namespace Module5.Player
{
    public class VideoPlayer360 : VideoPlayerBase, IVR360, IPointerClickHandler
    {
        [Inject] ISceneManager SceneManager {  get; set; }

        [SerializeField] MeshRenderer meshRenderer;

        public float Size
        {
            get
            {
                return meshRenderer != null ? meshRenderer.transform.localScale.x : 100f;
            }
            set
            {
                if (meshRenderer != null)
                {
                    meshRenderer.transform.localScale = new Vector3(value, value, value);
                }
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
            if (meshRenderer == null || meshRenderer.material == null) return;
            Material mat = meshRenderer.material;

            if (sphereType == VR360SphereType.Background)
            {
                mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
                mat.SetInt("_ZWrite", 0);
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Background;
            }
            else
            {
                mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
                mat.SetInt("_ZWrite", 1);
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
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

        [Header("360 Background Visual Settings")]
        [SerializeField] protected float normalExposure = 1.0f;
        [SerializeField] protected float pausedExposure = 0.5f;

        private Mesh _invertedMesh;
        private MeshCollider _meshCollider;

        protected override void Awake()
        {
            base.Awake();
            // 確保 VideoPlayer 的 RenderMode 為 MaterialOverride
            videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
            SetupInvertedMeshCollider();
            ApplySphereTypeToMaterial();
        }

        private void SetupInvertedMeshCollider()
        {
            if (meshRenderer == null) return;
            MeshFilter mf = meshRenderer.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) return;

            // 複製球體網格並反轉三角形，使法線朝向球心（空心薄殼）
            _invertedMesh = Instantiate(mf.sharedMesh);
            _invertedMesh.name = $"{mf.sharedMesh.name}_InvertedCollider";

            int[] triangles = _invertedMesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                (triangles[i], triangles[i + 1]) = (triangles[i + 1], triangles[i]);
            }
            _invertedMesh.triangles = triangles;
            _invertedMesh.RecalculateNormals();

            _meshCollider = meshRenderer.gameObject.GetComponent<MeshCollider>();
            if (_meshCollider == null)
            {
                _meshCollider = meshRenderer.gameObject.AddComponent<MeshCollider>();
            }
            _meshCollider.convex = false;
            _meshCollider.sharedMesh = _invertedMesh;

            // 確保 Sphere 子物件帶有點擊轉發器
            SphereClickHandler clickHandler = meshRenderer.gameObject.GetComponent<SphereClickHandler>();
            if (clickHandler == null)
            {
                clickHandler = meshRenderer.gameObject.AddComponent<SphereClickHandler>();
            }
            clickHandler.player = this;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            OnSphereClicked();
        }

        public virtual void OnSphereClicked()
        {
            TogglePlayPause();
        }

        protected override void UpdateUIState(bool isPlaying)
        {
            base.UpdateUIState(isPlaying);

            if (meshRenderer != null)
            {
                Material mat = Application.isPlaying ? meshRenderer.material : meshRenderer.sharedMaterial;
                if (mat != null)
                {
                    if (isPlaying)
                    {
                        mat.SetFloat("_Exposure", normalExposure);
                        mat.SetFloat("_ShowPlayIcon", 0f);
                    }
                    else
                    {
                        mat.SetFloat("_Exposure", pausedExposure);
                        mat.SetFloat("_ShowPlayIcon", showPlayButton ? 1f : 0f);
                        UpdateDecalOrientation(mat);
                    }
                }
            }
        }

        private void UpdateDecalOrientation(Material mat)
        {
            if (mat == null) return;

            Camera cam = (SceneManager != null && SceneManager.VirtualCamera != null) ? SceneManager.VirtualCamera : Camera.main;
            Vector3 camPos = (cam != null) ? cam.transform.position : transform.position;
            Vector3 camFwd = (cam != null) ? cam.transform.forward : Vector3.forward;
            Vector3 camUp = (cam != null) ? cam.transform.up : Vector3.up;

            Vector3 decalCenterDir = camFwd;
            if (_meshCollider != null && meshRenderer != null)
            {
                Ray ray = new Ray(camPos, camFwd);
                if (_meshCollider.Raycast(ray, out RaycastHit hit, 1000f))
                {
                    decalCenterDir = (hit.point - meshRenderer.transform.position).normalized;
                }
            }

            Vector3 decalUp = Vector3.ProjectOnPlane(camUp, decalCenterDir).normalized;
            if (decalUp.sqrMagnitude < 0.001f)
            {
                decalUp = Vector3.ProjectOnPlane(Vector3.up, decalCenterDir).normalized;
            }
            Vector3 decalRight = Vector3.Cross(decalUp, decalCenterDir).normalized;

            mat.SetVector("_DecalForward", new Vector4(decalCenterDir.x, decalCenterDir.y, decalCenterDir.z, 0f));
            mat.SetVector("_DecalRight", new Vector4(decalRight.x, decalRight.y, decalRight.z, 0f));
            mat.SetVector("_DecalUp", new Vector4(decalUp.x, decalUp.y, decalUp.z, 0f));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ApplySphereTypeToMaterial();
            if (isOverrideFov)
            {
                ApplyFovToCamera();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (isOverrideFov && SceneManager != null && SceneManager.VirtualCamera != null)
            {
                SyncVirtualCamera syncVirtualCamera = SceneManager.VirtualCamera.GetComponent<SyncVirtualCamera>();
                if (syncVirtualCamera != null)
                {
                    syncVirtualCamera.SetFovOverride(false);
                }
            }
        }

        protected override void OnVideoEnd(VideoPlayer source)
        {
            base.OnVideoEnd(source);
            // 接M5Event
            Debug.Log("OnVideoEnd未實作串接M5Event");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_invertedMesh != null)
            {
                Destroy(_invertedMesh);
                _invertedMesh = null;
            }
        }

        private class SphereClickHandler : MonoBehaviour, IPointerClickHandler
        {
            public VideoPlayer360 player;

            public void OnPointerClick(PointerEventData eventData)
            {
                if (player != null)
                {
                    player.OnSphereClicked();
                }
            }
        }
    }
}