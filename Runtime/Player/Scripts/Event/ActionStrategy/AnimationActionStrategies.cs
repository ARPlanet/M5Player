using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    public class AnimationPlayActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";
        public const string ParamClip = "clip";
        public const string ParamFadeLength = "fadeLength";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() },
            { ParamClip, string.Empty },
            { ParamFadeLength, 0.3f }
        };

        public IEnumerator Execute(M5Command command)
        {
            var modelRenderer = (ModelRenderer)command.Parameters[ParamTarget];
            if (modelRenderer != null)
            {
                string clipName = (string)command.Parameters[ParamClip];
                float fadeLength = (float)command.Parameters[ParamFadeLength];
                Debug.Log($"[Action {command.CommandType}][target={modelRenderer.gameObject.name}, clip={clipName}, fadeLength={fadeLength}]");
                modelRenderer.PlayAnimation(clipName, fadeLength);
            }
            yield break;
        }
    }

    public class AnimationPauseActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() }
        };

        public IEnumerator Execute(M5Command command)
        {
            var modelRenderer = (ModelRenderer)command.Parameters[ParamTarget];
            if (modelRenderer != null)
            {
                Debug.Log($"[Action {command.CommandType}][target={modelRenderer.gameObject.name}]");
                modelRenderer.PauseAnimation();
            }
            yield break;
        }
    }

    public class AnimationStopActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() }
        };

        public IEnumerator Execute(M5Command command)
        {
            var modelRenderer = (ModelRenderer)command.Parameters[ParamTarget];
            if (modelRenderer != null)
            {
                Debug.Log($"[Action {command.CommandType}][target={modelRenderer.gameObject.name}]");
                modelRenderer.StopAnimation();
            }
            yield break;
        }
    }
}
