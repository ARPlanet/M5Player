using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    public class ComponentChangeTextActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";
        public const string ParamText = "text";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() },
            { ParamText, string.Empty }
        };

        public IEnumerator Execute(M5Command command)
        {
            var textComp = (UnityEngine.UI.Text)command.Parameters[ParamTarget];
            if (textComp != null)
            {
                DynamicTextReplacer replacer = textComp.GetComponent<DynamicTextReplacer>();
                if (replacer != null)
                {
                    string text = (string)command.Parameters[ParamText];
                    Debug.Log($"[Action {command.CommandType}][target={textComp.gameObject.name}, text={text}]");
                    replacer.RawText = text;
                }
            }
            yield break;
        }
    }

    public class ComponentChangeImageActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";
        public const string ParamTexture = "texture";

        public virtual Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() },
            { ParamTexture, Guid.Empty.ToString() }
        };

        protected readonly IAssetLoaderManager _assetLoaderManager;

        public ComponentChangeImageActionStrategy(IAssetLoaderManager assetLoaderManager)
        {
            _assetLoaderManager = assetLoaderManager;
        }

        public virtual IEnumerator Execute(M5Command command)
        {
            var rawImage = (UnityEngine.UI.RawImage)command.Parameters[ParamTarget];
            if (rawImage != null)
            {
                object textureObj = command.Parameters[ParamTexture];
                Texture tex = null;
                if (textureObj is Texture t)
                {
                    tex = t;
                }
                else if (textureObj is Guid textureGuid && _assetLoaderManager != null)
                {
                    var task = _assetLoaderManager.LoadInstance(textureGuid);
                    while (!task.IsCompleted) yield return null;
                    var textureContainer = task.Result;
                    if (textureContainer != null && textureContainer.Obj is Texture loadedTex)
                    {
                        tex = loadedTex;
                    }
                }

                if (tex != null)
                {
                    Debug.Log($"[Action {command.CommandType}][target={rawImage.gameObject.name}, texture={tex.name}]");
                    rawImage.texture = tex;
                }
            }
            yield break;
        }
    }

    public class ComponentTextInputActionStrategy : IActionStrategy
    {
        public const string ParamInputField = "inputField";
        public const string ParamVariable = "variable";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamInputField, Guid.Empty.ToString() },
            { ParamVariable, string.Empty }
        };

        private readonly IVariableManager _variableManager;

        public ComponentTextInputActionStrategy(IVariableManager variableManager)
        {
            _variableManager = variableManager;
        }

        public IEnumerator Execute(M5Command command)
        {
            var inputField = (UnityEngine.UI.InputField)command.Parameters[ParamInputField];
            if (inputField != null)
            {
                string variableKey = (string)command.Parameters[ParamVariable];
                if (!string.IsNullOrEmpty(variableKey))
                {
                    Debug.Log($"[Action {command.CommandType}][inputField={inputField.gameObject.name}, variable={variableKey}, value={inputField.text}]");
                    _variableManager.SetValue(variableKey, inputField.text);
                }
            }
            yield break;
        }
    }
}
