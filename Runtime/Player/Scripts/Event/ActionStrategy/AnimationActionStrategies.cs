using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Command("Animation", "Play")]
    public class AnimationPlayActionStrategy : IActionStrategy
    {
        public const string CommandKey = "animation_play";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";
        public const string ParamClip = "clip";
        public const string ParamFadeLength = "fadeLength";
        public const string ParamWrapMode = "wrapMode";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<ModelRenderer>() },
            { ParamClip, ParameterDefinition.Create<string>(string.Empty) },
            { ParamFadeLength, ParameterDefinition.Create<float>(0.3f) },
            { ParamWrapMode, ParameterDefinition.Create<WrapMode>(WrapMode.Default) }
        };

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            var modelRenderer = (ModelRenderer)command.Parameters[ParamTarget];
            if (modelRenderer != null)
            {
                string clipName = (string)command.Parameters[ParamClip];
                float fadeLength = (float)command.Parameters[ParamFadeLength];
                WrapMode wrapMode = WrapMode.Default;
                if (command.Parameters.TryGetValue(ParamWrapMode, out object w) && w != null)
                {
                    if (w is WrapMode wm) wrapMode = wm;
                    else wrapMode = (WrapMode)System.Convert.ToInt32(w);
                }

                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={modelRenderer.gameObject.name}, clip={clipName}, fadeLength={fadeLength}, wrapMode={wrapMode}]");
                modelRenderer.PlayAnimation(clipName, fadeLength, wrapMode);
            }
            yield break;
        }
    }

    [Command("Animation", "Pause")]
    public class AnimationPauseActionStrategy : IActionStrategy
    {
        public const string CommandKey = "animation_pause";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<ModelRenderer>() }
        };

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            var modelRenderer = (ModelRenderer)command.Parameters[ParamTarget];
            if (modelRenderer != null)
            {
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={modelRenderer.gameObject.name}]");
                modelRenderer.PauseAnimation();
            }
            yield break;
        }
    }

    [Command("Animation", "Stop")]
    public class AnimationStopActionStrategy : IActionStrategy
    {
        public const string CommandKey = "animation_stop";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<ModelRenderer>() }
        };

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            var modelRenderer = (ModelRenderer)command.Parameters[ParamTarget];
            if (modelRenderer != null)
            {
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={modelRenderer.gameObject.name}]");
                modelRenderer.StopAnimation();
            }
            yield break;
        }
    }
}
