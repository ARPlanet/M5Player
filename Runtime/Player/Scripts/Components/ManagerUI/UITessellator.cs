using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    /// <summary>
    /// UGUI 網格細分組件 (UI Tessellation)。
    /// 透過 BaseMeshEffect 將 Quad 網格細分成 NxN 的頂點陣列，
    /// 使 UI 頂點著色器 (Vertex Shader) 的非線性變形（如螺旋旋轉、波動、擴散）呈現細膩的平滑曲面。
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Graphic))]
    [AddComponentMenu("UI/Effects/UI Tessellator")]
    public class UITessellator : BaseMeshEffect
    {
        [Tooltip("水平細分格數 (預設 8)")]
        [Range(1, 32)]
        [SerializeField] private int _subdivisionsX = 8;

        [Tooltip("垂直細分格數 (預設 8)")]
        [Range(1, 32)]
        [SerializeField] private int _subdivisionsY = 8;

        public int subdivisionsX
        {
            get => _subdivisionsX;
            set
            {
                int clamped = Mathf.Clamp(value, 1, 32);
                if (_subdivisionsX != clamped)
                {
                    _subdivisionsX = clamped;
                    if (graphic != null) graphic.SetVerticesDirty();
                }
            }
        }

        public int subdivisionsY
        {
            get => _subdivisionsY;
            set
            {
                int clamped = Mathf.Clamp(value, 1, 32);
                if (_subdivisionsY != clamped)
                {
                    _subdivisionsY = clamped;
                    if (graphic != null) graphic.SetVerticesDirty();
                }
            }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || vh.currentVertCount == 0) return;

            // 若皆為 1，代表保持原四邊形，不作細分
            if (_subdivisionsX <= 1 && _subdivisionsY <= 1) return;

            int vertCount = vh.currentVertCount;
            // 僅處理 4 的倍數頂點 (Quad 結構)，若為文字或其他複雜幾何體則保持原狀避免破壞
            if (vertCount % 4 != 0) return;

            UIVertex[] origVerts = new UIVertex[vertCount];
            for (int i = 0; i < vertCount; i++)
            {
                vh.PopulateUIVertex(ref origVerts[i], i);
            }

            vh.Clear();

            int quadCount = vertCount / 4;
            // 若為單一 Quad (一般 Image/RawImage)，完全依設定細分；
            // 若為多個 Quad (如九宮格 Sliced Image)，適度縮減細分以保護效能
            int subX = quadCount == 1 ? _subdivisionsX : Mathf.Clamp(_subdivisionsX / 2, 1, 4);
            int subY = quadCount == 1 ? _subdivisionsY : Mathf.Clamp(_subdivisionsY / 2, 1, 4);

            for (int q = 0; q < quadCount; q++)
            {
                int baseOffset = q * 4;
                UIVertex v0 = origVerts[baseOffset + 0]; // 左下
                UIVertex v1 = origVerts[baseOffset + 1]; // 左上
                UIVertex v2 = origVerts[baseOffset + 2]; // 右上
                UIVertex v3 = origVerts[baseOffset + 3]; // 右下

                int baseVertIndex = vh.currentVertCount;

                // 1. 生成細分頂點 (雙線性插值)
                for (int y = 0; y <= subY; y++)
                {
                    float ty = (float)y / subY;
                    for (int x = 0; x <= subX; x++)
                    {
                        float tx = (float)x / subX;
                        UIVertex vert = LerpUIVertex(v0, v1, v2, v3, tx, ty);
                        vh.AddVert(vert);
                    }
                }

                // 2. 生成三角形索引
                for (int y = 0; y < subY; y++)
                {
                    for (int x = 0; x < subX; x++)
                    {
                        int row1 = baseVertIndex + y * (subX + 1);
                        int row2 = baseVertIndex + (y + 1) * (subX + 1);

                        int i0 = row1 + x;         // 左下
                        int i1 = row2 + x;         // 左上
                        int i2 = row2 + x + 1;     // 右上
                        int i3 = row1 + x + 1;     // 右下

                        vh.AddTriangle(i0, i1, i2);
                        vh.AddTriangle(i0, i2, i3);
                    }
                }
            }
        }

        private static UIVertex LerpUIVertex(UIVertex v0, UIVertex v1, UIVertex v2, UIVertex v3, float tx, float ty)
        {
            UIVertex vert = new UIVertex();
            // 雙線性插值公式: Point = Lerp(Lerp(v0, v1, ty), Lerp(v3, v2, ty), tx)
            vert.position = Vector3.Lerp(Vector3.Lerp(v0.position, v1.position, ty), Vector3.Lerp(v3.position, v2.position, ty), tx);
            vert.normal = Vector3.Lerp(Vector3.Lerp(v0.normal, v1.normal, ty), Vector3.Lerp(v3.normal, v2.normal, ty), tx);
            vert.tangent = Vector4.Lerp(Vector4.Lerp(v0.tangent, v1.tangent, ty), Vector4.Lerp(v3.tangent, v2.tangent, ty), tx);
            vert.color = Color32.Lerp(Color32.Lerp(v0.color, v1.color, ty), Color32.Lerp(v3.color, v2.color, ty), tx);
            vert.uv0 = Vector4.Lerp(Vector4.Lerp(v0.uv0, v1.uv0, ty), Vector4.Lerp(v3.uv0, v2.uv0, ty), tx);
            vert.uv1 = Vector4.Lerp(Vector4.Lerp(v0.uv1, v1.uv1, ty), Vector4.Lerp(v3.uv1, v2.uv1, ty), tx);
            return vert;
        }
    }
}
