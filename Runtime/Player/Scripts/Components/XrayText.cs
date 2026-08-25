using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    public class XrayText : MonoBehaviour
    {
        Text text;
        Text Text
        {
            get
            {
                if(text == null) text = GetComponent<Text>();
                return text;
            }
        }

        [SerializeField] XrayType xrayType = XrayType.Mask;
        public XrayType XrayType
        {
            get => xrayType;
            set
            {
                xrayType = value;
                if (enabled) Text.material = Material;
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
                    materialMask = new Material(Shader.Find("UI/Default Font"));
                    materialMask.name = $"Xray Text Mask (Instance)";
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
                    materialObject = new Material(Shader.Find("UI/Default Font"));
                    materialObject.name = $"Xray Text (Instance)";
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

        private void Awake()
        {
            StencilId = stencilId;
        }

        private void OnEnable()
        {
            Text.material = Material;
        }

        private void OnDisable()
        {
            Text.material = null;
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