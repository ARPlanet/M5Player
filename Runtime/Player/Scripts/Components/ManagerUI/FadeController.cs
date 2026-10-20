using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
namespace Module5.Player
{
    public enum VoronoiType
    {
        F1,     // 距離最近點（平滑細胞感）
        F2MinusF1, // 第二近距離 - 最近距離（細胞邊界線感）
    }

    public class FadeController : MonoBehaviour, IFadeController
    {
        public enum NoiseMode
        {
            Particle,   // 粒子感消融（黑底散佈亮點）
            Blocky,     // 方塊感消融（格狀量化噪點）
            Voronoi,    // Voronoi 細胞感消融（有機和細胞樣式）
            Perlin,     // 原始 Perlin 平滑噪點
        }

        [Header("目標物件設定")]
        public List<GameObject> targetObjects = new List<GameObject>();

        [Header("動畫設定")]
        [SerializeField] private float _duration = 2.0f;
        public float duration { get => _duration; set => _duration = value; }

        [Header("效果表現設定")]
        public float expandScale = 1.0f;    // 消融時的擴散/縮放倍率
        public float spiralIntensity = 2.0f; // 消融時的旋轉強度
        [ColorUsage(true, true)] public Color edgeColor = new Color(0, 1, 1, 1); // 邊緣發光顏色
        [Range(0f, 0.5f)] public float edgeWidth = 0.1f; // 邊緣寬度
        [Tooltip("全域消融顆粒在螢幕上的基準像素大小 (預設 50)。數值越大，顆粒越粗，無論 3D 還是 UI 皆同步。")]
        public float baseGrainPixelSize = 50f;

        [Header("UI 網格細分設定 (Tessellation)")]
        [Tooltip("是否對 UI 物件啟用網格細分，使消融與旋轉位移呈現平滑曲面變形而非剛性四邊形")]
        public bool enableUITessellation = true;
        [Tooltip("水平細分格數 (建議 4 ~ 16)")]
        [Range(1, 32)] public int uiSubdivisionsX = 8;
        [Tooltip("垂直細分格數 (建議 4 ~ 16)")]
        [Range(1, 32)] public int uiSubdivisionsY = 8;

        [Header("Noise 生成設定")]
        public NoiseMode noiseMode = NoiseMode.Particle;
        public int noiseResolution = 256;
        public float noiseScale = 5.0f;

        [Header("粒子 Noise 設定 (NoiseMode = Particle)")]
        [Tooltip("粒子數量，越多粒子越密集")]
        public int particleCount = 2000;
        [Tooltip("每顆粒子的半徑 (像素)")]
        [Range(1, 16)] public int particleRadius = 3;

        [Header("方塊 Noise 設定 (NoiseMode = Blocky)")]
        [Tooltip("每個色塊的像素大小，數值越大方塊越明顯")]
        public int cellSize = 16;
        [Tooltip("色階數量，數值越小方塊邊緣越硬")]
        [Range(2, 32)] public int posterizeLevels = 4;

        [Header("Voronoi Noise 設定 (NoiseMode = Voronoi)")]
        [Tooltip("Voronoi 種子點數量，越少細胞越大，越多細胞越小")]
        public int voronoiCellCount = 64;
        [Tooltip("F1: 到最近點的距離（平滑細胞）\nF2-F1: 細胞邊界線（黑底紋線感）")]
        public VoronoiType voronoiType = VoronoiType.F1;
        [Tooltip("將距離分配到每個 Cell 的隨機亮度，讓每個細胞在消融時不同時段消失")]
        public bool voronoiRandomizeCells = true;

        [SerializeField] private Texture2D _generatedNoise;

        private class FadeTargetInfo
        {
            public Renderer renderer;
            public Graphic graphic; // UI 用
            public Material[] originalMaterials;
            public Material[] fadeMatInstances;            // 3D 用：每個 Sub-mesh 的材質實例
            public MaterialPropertyBlock[] propertyBlocks; // 用於動態更新 _Progress
            public Material uiMaterialInstance;            // UI 專用的材質實例
            public UITessellator uiTessellator;            // UI 細分效果組件實例
            public Vector4 uiCenter;                       // UI 自身中心點座標 (Canvas 本地座標 = UGUI 頂點 Object Space; xyz: 中心, w: 啟用標記)

            // UI / 3D 空間資訊
            public bool isScreen;
            public RectTransform rectTransform;
            public Transform canvasTransform;              // UGUI 頂點所在的 Object Space (Root Canvas，巢狀 Canvas 亦同)
        }

        private class FadeGroup
        {
            public List<FadeTargetInfo> targets = new List<FadeTargetInfo>();

            public void Reset()
            {
                targets.Clear();
            }
        }

        private class FadeProcess
        {
            public Coroutine coroutine;
            public bool isFadeIn;
            public float duration;

            public float expandScale;
            public float spiralIntensity;
            public Color edgeColor;
            public float edgeWidth;
            public float baseGrainPixelSize;

            public bool enableUITessellation;
            public int uiSubdivisionsX;
            public int uiSubdivisionsY;

            public List<GameObject> targetObjects = new List<GameObject>();
            public FadeGroup worldGroup = new FadeGroup();
            public FadeGroup screenGroup = new FadeGroup();
        }

        private List<FadeProcess> _activeProcesses = new List<FadeProcess>();

        [SerializeField] private Material _worldGatherMat;
        [SerializeField] private Material _standardGatherMat;
        [SerializeField] private Material _gltfPbrMetGatherMat;
        [SerializeField] private Material _gltfPbrSpecGatherMat;
        [SerializeField] private Material _gltfUnlitGatherMat;

        [SerializeField] private Material _uiTextGatherMat;
        [SerializeField] private Material _uiImageGatherMat;

        private static readonly int ProgressId = Shader.PropertyToID("_Progress");
        private static readonly int NoiseTexId = Shader.PropertyToID("_NoiseTex");
        private static readonly int NoiseTilingId = Shader.PropertyToID("_NoiseTiling");
        private static readonly int ExpandScaleId = Shader.PropertyToID("_ExpandScale");
        private static readonly int SpiralIntensityId = Shader.PropertyToID("_SpiralIntensity");
        private static readonly int EdgeColorId = Shader.PropertyToID("_EdgeColor");
        private static readonly int EdgeWidthId = Shader.PropertyToID("_EdgeWidth");
        private static readonly int BaseGrainPixelSizeId = Shader.PropertyToID("_BaseGrainPixelSize");
        private static readonly int ObjectCenterId = Shader.PropertyToID("_ObjectCenter");
        private static readonly int IsScreenSpaceId = Shader.PropertyToID("_IsScreenSpace");

        private static readonly Vector3[] _cornerBuffer = new Vector3[4];


        void Start()
        {
            RegenerateNoise();
        }

        public void SetTargetObjects(GameObject[] objects)
        {
            targetObjects.Clear();
            if (objects != null) targetObjects.AddRange(objects);
        }

        private Texture2D GenerateNoiseTexture()
        {
            Texture2D tex = null;
            switch (noiseMode)
            {
                case NoiseMode.Particle:
                    tex = NoiseGenerator.CreateParticleNoiseTexture(noiseResolution, noiseResolution, particleCount, particleRadius);
                    break;
                case NoiseMode.Blocky:
                    tex = NoiseGenerator.CreateBlockyNoiseTexture(noiseResolution, noiseResolution, noiseScale, cellSize, posterizeLevels);
                    break;
                case NoiseMode.Voronoi:
                    tex = NoiseGenerator.CreateVoronoiNoiseTexture(noiseResolution, noiseResolution, voronoiCellCount, voronoiType, voronoiRandomizeCells);
                    break;
                case NoiseMode.Perlin:
                    tex = NoiseGenerator.CreatePerlinNoiseTexture(noiseResolution, noiseResolution, noiseScale);
                    break;
            }

            if (tex != null)
            {
                // 強制設定 Repeat 與 Bilinear 濾鏡，解決座標原點切割與滑動時的邊緣問題
                tex.wrapMode = TextureWrapMode.Repeat;
                tex.filterMode = FilterMode.Bilinear;
                tex.Apply();
            }
            return tex;
        }

        private void RegenerateNoise()
        {
            if (_generatedNoise != null) Destroy(_generatedNoise);
            _generatedNoise = GenerateNoiseTexture();
            // 設定全域參數，供所有 Gather Shader 共用
            Shader.SetGlobalTexture(NoiseTexId, _generatedNoise);
        }

        private void SetupProcessTargets(FadeProcess process)
        {
            process.worldGroup.Reset();
            process.screenGroup.Reset();

            // 強制刷新 Canvas / Layout，確保 Root Canvas 尺寸、Anchor、LayoutGroup 已計算完成
            // 否則 GetWorldCorners 會取得舊的 rect，導致中心點隨 Anchor 偏移
            Canvas.ForceUpdateCanvases();

            foreach (var obj in process.targetObjects)
            {
                if (obj == null) continue;

                // 1. 處理 3D Renderers (自動識別是否掛載於 Screen Canvas 或 Camera 底下)
                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
                foreach (var r in renderers)
                {
                    bool isScreen = ResolveScreenSpace(r);
                    var info = ProcessRenderer(r, isScreen);
                    FadeGroup group = isScreen ? process.screenGroup : process.worldGroup;
                    group.targets.Add(info);
                }

                // 2. 處理 UI Graphics
                Graphic[] graphics = obj.GetComponentsInChildren<Graphic>(true);
                foreach (var g in graphics)
                {
                    // 判斷 World / Screen：
                    // UICanvas 為專案業務邏輯最高真相來源：
                    // - Screen: 螢幕 UI (Player 下為 ScreenSpaceCamera，Editor 下為貼於相機前模擬的特例)
                    // - World: 3D 世界 UI (Player 與 Editor 下皆為 WorldSpaceCamera)
                    UICanvas uiCanvas = g.GetComponentInParent<UICanvas>(true);
                    bool isScreen = false;
                    if (uiCanvas != null)
                    {
                        isScreen = (uiCanvas.CanvasType == CanvasType.Screen);
                    }
                    else
                    {
                        Canvas canvas = g.canvas;
                        if (canvas == null) canvas = g.GetComponentInParent<Canvas>(true);
                        isScreen = (canvas != null && (canvas.renderMode == RenderMode.ScreenSpaceOverlay || canvas.renderMode == RenderMode.ScreenSpaceCamera));
                    }

                    var info = ProcessGraphic(g, isScreen);

                    // 若啟用 UI 細分且為圖片 (Image / RawImage)，動態掛載 UITessellator
                    if (process.enableUITessellation && (g is Image || g is RawImage))
                    {
                        var tess = g.gameObject.AddComponent<UITessellator>();
                        tess.subdivisionsX = process.uiSubdivisionsX;
                        tess.subdivisionsY = process.uiSubdivisionsY;
                        info.uiTessellator = tess;
                        g.SetVerticesDirty();
                    }

                    FadeGroup group = isScreen ? process.screenGroup : process.worldGroup;
                    group.targets.Add(info);

                    ResolveUISpace(info, g, isScreen);
                    RefreshUICenter(info);
                    info.uiMaterialInstance.SetVector(ObjectCenterId, info.uiCenter);
                }
            }
        }

        /// <summary>
        /// 解析 UI 所在的 Canvas (UGUI 頂點的 Object Space)
        /// </summary>
        private void ResolveUISpace(FadeTargetInfo info, Graphic g, bool isScreen)
        {
            info.isScreen = isScreen;
            info.rectTransform = g.rectTransform;

            Canvas canvas = g.canvas;
            if (canvas == null) canvas = g.GetComponentInParent<Canvas>(true);

            // UGUI 頂點 (含巢狀 Canvas) 皆以 Root Canvas 的矩陣繪製，
            // 因此 Object Space = Root Canvas 本地座標，而非最近的巢狀 Canvas
            Canvas root = canvas != null ? (canvas.rootCanvas != null ? canvas.rootCanvas : canvas) : null;
            info.canvasTransform = root != null ? root.transform : null;
        }

        /// <summary>
        /// 以 Canvas 本地座標計算 UI 幾何中心。
        /// Canvas 本地座標與 Shader 中的 positionOS 同一空間，不受攝影機移動 / Canvas 世界位置影響。
        /// </summary>
        private static void RefreshUICenter(FadeTargetInfo info)
        {
            if (info.rectTransform == null || info.canvasTransform == null)
            {
                info.uiCenter = Vector4.zero; // w = 0：Shader 回退為 Object 原點
                return;
            }
            info.rectTransform.GetWorldCorners(_cornerBuffer);
            Vector3 worldCenter = (_cornerBuffer[0] + _cornerBuffer[2]) * 0.5f;
            Vector3 localCenter = info.canvasTransform.InverseTransformPoint(worldCenter);
            info.uiCenter = new Vector4(localCenter.x, localCenter.y, localCenter.z, 1.0f);
        }

        /// <summary>
        /// 判斷組件是否掛載於 Screen UICanvas 或 Camera 底下 (用於 Shader 噪點取樣模式選擇)
        /// </summary>
        private static bool ResolveScreenSpace(Component comp)
        {
            if (comp == null) return false;

            // 1. 檢查是否位於 UICanvas 底下且 CanvasType == Screen
            UICanvas uiCanvas = comp.GetComponentInParent<UICanvas>(true);
            if (uiCanvas != null && uiCanvas.CanvasType == CanvasType.Screen)
            {
                return true;
            }

            // 2. 檢查是否位於原生 ScreenSpaceCanvas 底下
            Canvas c = comp.GetComponentInParent<Canvas>(true);
            if (c != null && (c.renderMode == RenderMode.ScreenSpaceOverlay || c.renderMode == RenderMode.ScreenSpaceCamera))
            {
                return true;
            }

            // 3. 檢查是否掛載於 Camera (例如 GameCamera 或 MainCamera) 底下
            Camera cam = comp.GetComponentInParent<Camera>(true);
            if (cam != null)
            {
                return true;
            }

            return false;
        }

        private FadeTargetInfo ProcessRenderer(Renderer r, bool isScreen)
        {
            var info = new FadeTargetInfo();
            info.renderer = r;
            info.isScreen = isScreen;
            info.originalMaterials = r.sharedMaterials;
            int matCount = r.sharedMaterials.Length;
            info.propertyBlocks = new MaterialPropertyBlock[matCount];
            Material[] fadeMats = new Material[matCount];
            info.fadeMatInstances = new Material[matCount];

            for (int i = 0; i < matCount; i++)
            {
                Material origMat = r.sharedMaterials[i];
                info.propertyBlocks[i] = new MaterialPropertyBlock();
                Material gatherBase = GetGatherBaseForShader(origMat);
                Material inst = new Material(gatherBase);
                info.fadeMatInstances[i] = inst;
                fadeMats[i] = inst;

                inst.SetTexture(NoiseTexId, _generatedNoise);
                if (origMat != null) CopyMaterialProperties(origMat, inst);
            }
            r.sharedMaterials = fadeMats;
            return info;
        }

        private Material GetGatherBaseForShader(Material m)
        {
            if (m == null) return _worldGatherMat;
            string s = m.shader.name;
            
            // Built-in glTFast
            if (s.Contains("glTF/PbrMetallicRoughness") || s.Contains("glTFPbrMetallicRoughness")) return _gltfPbrMetGatherMat;
            if (s.Contains("glTF/PbrSpecularGlossiness") || s.Contains("glTFPbrSpecularGlossiness")) return _gltfPbrSpecGatherMat;
            if (s.Contains("glTF/Unlit") || s.Contains("glTFUnlit")) return _gltfUnlitGatherMat;

            // URP glTFast (ShaderGraph)
            if (s.Contains("glTF-pbrMetallicRoughness")) return _gltfPbrMetGatherMat;
            if (s.Contains("glTF-pbrSpecularGlossiness")) return _gltfPbrSpecGatherMat;
            if (s.Contains("glTF-unlit")) return _gltfUnlitGatherMat;

            // Universal Render Pipeline / Built-in Standard
            if (s.Contains("Standard") || s.Contains("Universal Render Pipeline/Lit")) return _standardGatherMat;
            if (s.Contains("Universal Render Pipeline/Unlit")) return _worldGatherMat;

            return _worldGatherMat;
        }

        private void CopyMaterialProperties(Material origMat, Material inst)
        {
            // 複製 Shader Keywords (Shader Features) 與 Render Queue
            inst.shaderKeywords = origMat.shaderKeywords;
            inst.renderQueue = origMat.renderQueue;

            // 強制開啟 Alpha Clip 以支援消融效果
            inst.EnableKeyword("_ALPHATEST_ON");
            if (inst.HasProperty("alphaCutoff")) inst.SetFloat("alphaCutoff", 0.5f);

            Texture mainTex = origMat.mainTexture;
            Vector4 mainST = new Vector4(1, 1, 0, 0);
            if (mainTex != null)
                mainST = new Vector4(origMat.mainTextureScale.x, origMat.mainTextureScale.y, origMat.mainTextureOffset.x, origMat.mainTextureOffset.y);
            else
            {
                foreach (var n in new[] { "baseColorTexture", "_BaseMap", "_MainTex" })
                {
                    if (origMat.HasProperty(n) && origMat.GetTexture(n) != null)
                    {
                        mainTex = origMat.GetTexture(n);
                        var s = origMat.GetTextureScale(n);
                        var o = origMat.GetTextureOffset(n);
                        mainST = new Vector4(s.x, s.y, o.x, o.y); break;
                    }
                }
            }
            if (mainTex != null) { TrySetTex(inst, "baseColorTexture", mainTex, mainST); TrySetTex(inst, "_MainTex", mainTex, mainST); }

            Color mainColor = Color.white;
            foreach (var n in new[] { "baseColorFactor", "_BaseColor", "_Color" })
                if (origMat.HasProperty(n)) { mainColor = origMat.GetColor(n); break; }
            TrySetColor(inst, "baseColorFactor", mainColor); TrySetColor(inst, "_Color", mainColor);

            foreach (var n in new[] { "normalTexture", "_BumpMap" })
                if (origMat.HasProperty(n) && origMat.GetTexture(n) != null) TrySetTex(inst, n, origMat.GetTexture(n), mainST);

            if (origMat.HasProperty("metallicRoughnessTexture")) TrySetTex(inst, "metallicRoughnessTexture", origMat.GetTexture("metallicRoughnessTexture"), mainST);
            else if (origMat.HasProperty("_MetallicGlossMap")) TrySetTex(inst, "_MetallicGlossMap", origMat.GetTexture("_MetallicGlossMap"), mainST);

            if (origMat.HasProperty("metallicFactor")) TrySetFloat(inst, "metallicFactor", origMat.GetFloat("metallicFactor"));
            if (origMat.HasProperty("_Metallic")) TrySetFloat(inst, "_Metallic", origMat.GetFloat("_Metallic"));

            // 複製 Occlusion (遮蔽) 屬性
            foreach (var n in new[] { "occlusionTexture", "_OcclusionMap" })
                if (origMat.HasProperty(n) && origMat.GetTexture(n) != null) TrySetTex(inst, n, origMat.GetTexture(n), mainST);
            foreach (var n in new[] { "occlusionTexture_strength", "_OcclusionStrength" })
                if (origMat.HasProperty(n)) TrySetFloat(inst, n, origMat.GetFloat(n));

            // 複製 Emissive (自發光) 屬性
            foreach (var n in new[] { "emissiveTexture", "_EmissionMap" })
                if (origMat.HasProperty(n) && origMat.GetTexture(n) != null) TrySetTex(inst, n, origMat.GetTexture(n), mainST);
            foreach (var n in new[] { "emissiveFactor", "_EmissionColor" })
                if (origMat.HasProperty(n)) TrySetColor(inst, n, origMat.GetColor(n));
        }

        private static void TrySetTex(Material m, string prop, Texture tex, Vector4 st) { m.SetTexture(prop, tex); m.SetVector(prop + "_ST", st); }
        private static void TrySetColor(Material m, string prop, Color col) { m.SetColor(prop, col); }
        private static void TrySetFloat(Material m, string prop, float val) { m.SetFloat(prop, val); }

        private FadeTargetInfo ProcessGraphic(Graphic g, bool isScreen)
        {
            var info = new FadeTargetInfo();
            info.graphic = g;
            info.originalMaterials = new Material[] { g.material };
            Material baseMat = (g is Text) ? _uiTextGatherMat : _uiImageGatherMat;
            info.uiMaterialInstance = new Material(baseMat);

            // 使用 Float 屬性代替不穩定的 Keyword
            info.uiMaterialInstance.SetFloat("_IsScreenSpace", isScreen ? 1.0f : 0.0f);
            //Debug.Log($"[FadeController] UI: {g.name}, isScreen: {isScreen}, mode: {g.canvas?.renderMode}");

            info.uiMaterialInstance.mainTexture = g.mainTexture;
            info.uiMaterialInstance.SetColor("_Color", g.color);
            info.uiMaterialInstance.SetTexture(NoiseTexId, _generatedNoise);
            g.material = info.uiMaterialInstance;
            return info;
        }

        private void RestoreTargetInfo(FadeTargetInfo info)
        {
            if (info.renderer != null)
            {
                info.renderer.sharedMaterials = info.originalMaterials;
                for (int i = 0; i < info.propertyBlocks.Length; i++)
                    info.renderer.SetPropertyBlock(null, i);
                if (info.fadeMatInstances != null)
                    foreach (var m in info.fadeMatInstances) if (m != null) Destroy(m);
            }
            if (info.uiTessellator != null)
            {
                Destroy(info.uiTessellator);
                info.uiTessellator = null;
                if (info.graphic != null)
                {
                    info.graphic.SetVerticesDirty();
                }
            }
            if (info.graphic != null)
            {
                info.graphic.material = info.originalMaterials[0];
                if (info.uiMaterialInstance != null) Destroy(info.uiMaterialInstance);
            }
        }

        private void RestoreGroup(FadeGroup group)
        {
            foreach (var info in group.targets)
            {
                RestoreTargetInfo(info);
            }
            group.Reset();
        }

        private void RestoreFadeTargets(FadeProcess process)
        {
            RestoreGroup(process.worldGroup);
            RestoreGroup(process.screenGroup);
        }

        private void SetGroupRenderersEnabled(FadeGroup group, bool enabled)
        {
            foreach (var info in group.targets)
            {
                if (info.renderer != null)
                {
                    info.renderer.enabled = enabled;
                }
                if (info.graphic != null)
                {
                    info.graphic.enabled = enabled;
                }
            }
        }

        private void RemoveAndRestoreObjectFromProcess(FadeProcess process, GameObject rootObj)
        {
            process.targetObjects.Remove(rootObj);
            
            Renderer[] renderers = rootObj.GetComponentsInChildren<Renderer>(true);
            Graphic[] graphics = rootObj.GetComponentsInChildren<Graphic>(true);
            
            HashSet<Renderer> rndSet = new HashSet<Renderer>(renderers);
            HashSet<Graphic> grSet = new HashSet<Graphic>(graphics);
            
            for (int i = process.worldGroup.targets.Count - 1; i >= 0; i--)
            {
                var info = process.worldGroup.targets[i];
                if ((info.renderer != null && rndSet.Contains(info.renderer)) || 
                    (info.graphic != null && grSet.Contains(info.graphic)))
                {
                    RestoreTargetInfo(info);
                    process.worldGroup.targets.RemoveAt(i);
                }
            }
            for (int i = process.screenGroup.targets.Count - 1; i >= 0; i--)
            {
                var info = process.screenGroup.targets[i];
                if ((info.renderer != null && rndSet.Contains(info.renderer)) ||
                    (info.graphic != null && grSet.Contains(info.graphic)))
                {
                    RestoreTargetInfo(info);
                    process.screenGroup.targets.RemoveAt(i);
                }
            }
        }

        private void StartNewFadeProcess(bool isFadeIn)
        {
            FadeProcess newProcess = new FadeProcess();
            newProcess.isFadeIn = isFadeIn;
            newProcess.duration = this.duration;
            newProcess.expandScale = this.expandScale;
            newProcess.spiralIntensity = this.spiralIntensity;
            newProcess.edgeColor = this.edgeColor;
            newProcess.edgeWidth = this.edgeWidth;
            newProcess.baseGrainPixelSize = this.baseGrainPixelSize;
            newProcess.enableUITessellation = this.enableUITessellation;
            newProcess.uiSubdivisionsX = this.uiSubdivisionsX;
            newProcess.uiSubdivisionsY = this.uiSubdivisionsY;
            newProcess.targetObjects = new List<GameObject>(this.targetObjects);
            
            if (_generatedNoise == null) RegenerateNoise();

            foreach(var obj in newProcess.targetObjects)
            {
                if (obj == null) continue;
                foreach(var process in _activeProcesses)
                {
                    RemoveAndRestoreObjectFromProcess(process, obj);
                }
            }
            
            _activeProcesses.Add(newProcess);
            newProcess.coroutine = StartCoroutine(AnimateFadeProcess(newProcess));
        }

        [ContextMenu("Start Fade In")]
        public void StartFadeIn() { StartNewFadeProcess(true); }
        [ContextMenu("Start Fade Out")]
        public void StartFadeOut() { StartNewFadeProcess(false); }

        IEnumerator AnimateFadeProcess(FadeProcess process)
        {
            if (process.isFadeIn)
            {
                // 修正手機端場景Awake 物件無法同步完成初始化造成的渲染問題
                bool hasInactive = false;
                foreach (var obj in process.targetObjects)
                {
                    if (obj == null) continue;
                    if (!obj.activeSelf)
                    {
                        hasInactive = true;
                        obj.SetActive(true);
                    }
                    CanvasRenderer[] canvasRenderers = obj.GetComponentsInChildren<CanvasRenderer>(true);
                    foreach (var r in canvasRenderers) r.SetAlpha(0f);
                    Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
                    foreach (var r in renderers)
                    {
                        r.enabled = true;
                        r.forceRenderingOff = true;
                    }
                    Graphic[] graphics = obj.GetComponentsInChildren<Graphic>(true);
                    foreach (var g in graphics)
                    {
                        g.enabled = true;
                    }
                }

                if (hasInactive)
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            SetupProcessTargets(process);

            if (process.isFadeIn)
            {
                // 全域消融進度：1.0 = 完全消融擴散 (不可見)，0.0 = 完全凝聚顯現 (實體)
                UpdateFadeValue(process, 1f);
                SetGroupRenderersEnabled(process.worldGroup, true);
                SetGroupRenderersEnabled(process.screenGroup, true);

                foreach (var obj in process.targetObjects)
                {
                    if (obj == null) continue;
                    CanvasRenderer[] canvasRenderers = obj.GetComponentsInChildren<CanvasRenderer>(true);
                    foreach (var r in canvasRenderers) r.SetAlpha(1f);
                    Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
                    foreach (var r in renderers) r.forceRenderingOff = false;
                }
            }
            else
            {
                UpdateFadeValue(process, 0f);
            }

            float elapsed = 0;
            while (elapsed < process.duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / process.duration);
                // 淡入 (FadeIn): 1.0 -> 0.0 (消融 -> 顯現)
                // 淡出 (FadeOut): 0.0 -> 1.0 (實體 -> 消融)
                float currentProgress = process.isFadeIn ? (1f - t) : t;
                UpdateFadeValue(process, currentProgress);
                yield return null;
            }
            UpdateFadeValue(process, process.isFadeIn ? 0f : 1f);

            if (!process.isFadeIn)
            {
                // 方案A：僅停用渲染組件，避免還原材質時物件顯形；保持 GameObject Active，避免干擾邏輯/音訊/光源/事件
                SetGroupRenderersEnabled(process.worldGroup, false);
                SetGroupRenderersEnabled(process.screenGroup, false);
            }

            RestoreFadeTargets(process);

            _activeProcesses.Remove(process);
        }

        private void UpdateFadeValue(FadeProcess process, float progress)
        {
            UpdateGroupVisuals(process.worldGroup, progress, process);
            UpdateGroupVisuals(process.screenGroup, progress, process);
        }

        private void UpdateGroupVisuals(FadeGroup group, float progress, FadeProcess process)
        {
            foreach (var info in group.targets)
            {
                if (info.renderer != null)
                {
                    for (int i = 0; i < info.propertyBlocks.Length; i++)
                    {
                        info.propertyBlocks[i].SetFloat(ProgressId, progress);
                        info.propertyBlocks[i].SetFloat(NoiseTilingId, 1.0f);
                        info.propertyBlocks[i].SetFloat(ExpandScaleId, process.expandScale);
                        info.propertyBlocks[i].SetFloat(SpiralIntensityId, process.spiralIntensity);
                        info.propertyBlocks[i].SetColor(EdgeColorId, process.edgeColor);
                        info.propertyBlocks[i].SetFloat(EdgeWidthId, process.edgeWidth);
                        info.propertyBlocks[i].SetFloat(BaseGrainPixelSizeId, process.baseGrainPixelSize);
                        info.propertyBlocks[i].SetFloat(IsScreenSpaceId, info.isScreen ? 1.0f : 0.0f);
                        info.renderer.SetPropertyBlock(info.propertyBlocks[i], i);
                    }
                }
                if (info.graphic != null && info.uiMaterialInstance != null)
                {
                    RefreshUICenter(info); // 每幀刷新 Canvas 本地中心，跟隨 Layout 變化

                    var inst = info.uiMaterialInstance;
                    inst.SetFloat(ProgressId, progress);
                    inst.SetFloat(NoiseTilingId, 1.0f);
                    inst.SetFloat(ExpandScaleId, process.expandScale);
                    inst.SetFloat(SpiralIntensityId, process.spiralIntensity);
                    inst.SetColor(EdgeColorId, process.edgeColor);
                    inst.SetFloat(EdgeWidthId, process.edgeWidth);
                    inst.SetFloat(BaseGrainPixelSizeId, process.baseGrainPixelSize);
                    inst.SetVector(ObjectCenterId, info.uiCenter);
                }
            }
        }
    }

    public static class NoiseGenerator
    {
        public static Texture2D CreateVoronoiNoiseTexture(int width, int height, int cellCount = 64, VoronoiType type = VoronoiType.F1, bool randomizeCells = true)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Repeat;

            Vector2[] seeds = new Vector2[cellCount];
            float[] cellBrightness = new float[cellCount];
            for (int i = 0; i < cellCount; i++)
            {
                seeds[i] = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
                cellBrightness[i] = randomizeCells ? Random.Range(0f, 1f) : 0f;
            }

            Color[] pixels = new Color[width * height];
            float maxDist = Mathf.Sqrt(0.5f * 0.5f + 0.5f * 0.5f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float u = (float)x / width;
                    float v = (float)y / height;

                    float f1 = float.MaxValue;
                    float f2 = float.MaxValue;
                    int nearestCell = 0;

                    for (int i = 0; i < cellCount; i++)
                    {
                        float dx = u - seeds[i].x;
                        float dy = v - seeds[i].y;
                        if (dx > 0.5f) dx -= 1f; else if (dx < -0.5f) dx += 1f;
                        if (dy > 0.5f) dy -= 1f; else if (dy < -0.5f) dy += 1f;

                        float dist = Mathf.Sqrt(dx * dx + dy * dy);
                        if (dist < f1) { f2 = f1; f1 = dist; nearestCell = i; }
                        else if (dist < f2) { f2 = dist; }
                    }

                    float sample = (type == VoronoiType.F2MinusF1) ? Mathf.Clamp01((f2 - f1) / maxDist) : Mathf.Clamp01(f1 / maxDist);
                    if (randomizeCells) sample = Mathf.Clamp01(sample * 0.4f + cellBrightness[nearestCell] * 0.6f);
                    pixels[y * width + x] = new Color(sample, sample, sample, 1f);
                }
            }
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        public static Texture2D CreateParticleNoiseTexture(int width, int height, int particleCount = 2000, int radius = 3)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Repeat;
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.black;

            for (int p = 0; p < particleCount; p++)
            {
                int cx = Random.Range(0, width);
                int cy = Random.Range(0, height);
                float brightness = Random.Range(0.05f, 1.0f);
                for (int dy = -radius; dy <= radius; dy++)
                {
                    for (int dx = -radius; dx <= radius; dx++)
                    {
                        if (dx * dx + dy * dy > radius * radius) continue;
                        float dist = Mathf.Sqrt(dx * dx + dy * dy);
                        float falloff = 1.0f - Mathf.SmoothStep(0f, radius, dist);
                        int px = (cx + dx + width) % width;
                        int py = (cy + dy + height) % height;
                        int idx = py * width + px;
                        float final = brightness * falloff;
                        if (final > pixels[idx].r) pixels[idx] = new Color(final, final, final, 1f);
                    }
                }
            }
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        public static Texture2D CreateBlockyNoiseTexture(int width, int height, float scale, int cellSize = 16, int posterizeLevels = 4)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Repeat;
            Color[] pixels = new Color[width * height];
            float ox = Random.Range(0f, 1000f), oy = Random.Range(0f, 1000f);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float sample = Mathf.PerlinNoise(ox + (x / cellSize * cellSize) / (float)width * scale, oy + (y / cellSize * cellSize) / (float)height * scale);
                    sample = Mathf.Round(sample * (posterizeLevels - 1)) / (posterizeLevels - 1);
                    pixels[y * width + x] = new Color(sample, sample, sample, 1f);
                }
            }
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        public static Texture2D CreatePerlinNoiseTexture(int width, int height, float scale)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Repeat;
            Color[] pixels = new Color[width * height];
            float ox = Random.Range(0f, 1000f), oy = Random.Range(0f, 1000f);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float sample = Mathf.PerlinNoise(ox + (float)x / width * scale, oy + (float)y / height * scale);
                    pixels[y * width + x] = new Color(sample, sample, sample, 1f);
                }
            }
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }
    }
}
