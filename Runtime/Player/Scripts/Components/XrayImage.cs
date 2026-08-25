using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Module5.Player
{
    public enum StencilComparison
    {
        Never = 1,
        Less = 2,
        Equal = 3,
        LEqual = 4,
        Greater = 5,
        NotEqual = 6,
        GEqual = 7,
        Always = 8,
    }

    public enum XrayType
    {
        Mask,
        Object
    }

    public class XrayImage : MonoBehaviour
    {
        RawImage rawImage;
        RawImage RawImage
        {
            get
            {
                if(rawImage == null) rawImage = GetComponent<RawImage>();
                return rawImage;
            }
        }

        [SerializeField] XrayType xrayType = XrayType.Mask;
        public XrayType XrayType
        {
            get => xrayType;
            set
            {
                xrayType = value;
                if(enabled) RawImage.material = Material;
                StencilId = stencilId;
            }
        }

        Material materialMask;
        Material MaterialMask
        {
            get
            {
                if (materialMask == null)
                {
                    materialMask = new Material(Shader.Find("UI/Default"));
                    materialMask.name = $"Xray Image Mask (Instance)";
                    materialMask.SetFloat("_UseUIAlphaClip", 1.0f);
                    materialMask.EnableKeyword("UNITY_UI_ALPHACLIP");
                    materialMask.SetFloat("_StencilComp", 8.0f);
                    materialMask.SetFloat("_StencilOp", 2.0f);
                    materialMask.renderQueue = 2999;
                }
                return materialMask;
            }
        }

        Material materialObject;
        Material MaterialObject
        {
            get
            {
                if (materialObject == null)
                {
                    materialObject = new Material(Shader.Find("UI/Default"));
                    materialObject.name = $"Xray Image (Instance)";
                    materialObject.SetFloat("_StencilComp", 3.0f);
                    materialObject.SetFloat("_StencilOp", 0f);
                }
                return materialObject;
            }
        }
        Material Material
        {
            get
            {
                return xrayType switch
                {
                    XrayType.Mask => MaterialMask,
                    XrayType.Object => MaterialObject,
                    _ => materialMask
                };
            }
        }
        [SerializeField] int stencilId; // 0-255
        public int StencilId
        {
            get
            {
                stencilId = (int)Material.GetFloat("_Stencil");
                return stencilId;
            }
            set
            {
                stencilId = value;
                Material.SetFloat("_Stencil", value);
            }
        }
        //[SerializeField] StencilComparison comparison;
        //public StencilComparison Comparison
        //{
        //    get
        //    {
        //        comparison = (StencilComparison)Material.GetFloat("_StencilComp");
        //        return comparison;
        //    }
        //    set
        //    {
        //        comparison = value;
        //        Material.SetFloat("_StencilComp", (float)value);
        //    }
        //}

        private void Awake()
        {
            StencilId = stencilId;
        }

        private void OnEnable()
        {
            RawImage.material = Material;
        }

        private void OnDisable()
        {
            RawImage.material = null;
        }

        private void OnDestroy()
        {
            if (materialMask != null) Destroy(materialMask);
            if (materialObject != null) Destroy(materialObject);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            StencilId = stencilId;
        }
#endif
    }
}