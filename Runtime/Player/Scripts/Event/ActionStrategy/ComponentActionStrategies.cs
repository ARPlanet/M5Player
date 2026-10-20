using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Command("Component", "Change Text")]
    public class ComponentChangeTextActionStrategy : IActionStrategy
    {
        public const string CommandKey = "component_change_text";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";
        public const string ParamText = "text";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<UnityEngine.UI.Text>() },
            { ParamText, ParameterDefinition.Create<string>(string.Empty) }
        };

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            if (ActionStrategyHelper.TryGetComponent<UnityEngine.UI.Text>(command, ParamTarget, out var textComp) && textComp != null)
            {
                command.Parameters.TryGetValue(ParamText, out object rawText);
                string text = ValueFormatter.FormatValueAsString(rawText);
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={textComp.gameObject.name}, text={text}]");

                DynamicTextReplacer replacer = textComp.GetComponent<DynamicTextReplacer>();
                if (replacer != null)
                {
                    replacer.RawText = text;
                }
                else
                {
                    textComp.text = text;
                }
            }
            yield break;
        }
    }

    [Command("Component", "Change Image")]
    public class ComponentChangeImageActionStrategy : IActionStrategy
    {
        public const string CommandKey = "component_change_image";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";
        public const string ParamTexture = "texture";

        public virtual IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<UnityEngine.UI.RawImage>() },
            { ParamTexture, ParameterDefinition.CreateObject<Texture>() }
        };

        protected readonly IAssetLoaderManager _assetLoaderManager;

        public ComponentChangeImageActionStrategy(IAssetLoaderManager assetLoaderManager)
        {
            _assetLoaderManager = assetLoaderManager;
        }

        public virtual IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            var rawImage = (UnityEngine.UI.RawImage)command.Parameters[ParamTarget];
            if (rawImage != null)
            {
                Texture tex = (Texture)command.Parameters[ParamTexture];
                string name = tex != null ? tex.name : "null";
                
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={rawImage.gameObject.name}, texture={name}]");
                rawImage.texture = tex;
            }
            yield break;
        }
    }

    [Command("Component", "Text Input")]
    public class ComponentTextInputActionStrategy : IActionStrategy
    {
        public const string CommandKey = "component_text_input";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamInputField = "inputField";
        public const string ParamVariable = "variable";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamInputField, ParameterDefinition.CreateObject<UnityEngine.UI.InputField>() },
            { ParamVariable, ParameterDefinition.CreateVariableKey() }
        };

        private readonly IVariableManager _variableManager;

        public ComponentTextInputActionStrategy(IVariableManager variableManager)
        {
            _variableManager = variableManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            var inputField = (UnityEngine.UI.InputField)command.Parameters[ParamInputField];
            if (inputField != null)
            {
                string variableKey = (string)command.Parameters[ParamVariable];
                if (!string.IsNullOrEmpty(variableKey))
                {
                    Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][inputField={inputField.gameObject.name}, variable={variableKey}, value={inputField.text}]");
                    _variableManager.SetValue(variableKey, inputField.text);
                }
            }
            yield break;
        }
    }
}
