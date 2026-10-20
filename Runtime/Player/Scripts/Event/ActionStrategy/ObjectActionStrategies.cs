using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Module5.Player
{
    [Command("Object", "Move")]
    public class ObjectMoveActionStrategy : IActionStrategy
    {
        public const string CommandKey = "object_move";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";
        public const string ParamToX = "toX";
        public const string ParamToY = "toY";
        public const string ParamToZ = "toZ";
        public const string ParamDuration = "duration";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<Transform>() },
            { ParamToX, ParameterDefinition.Create<float>(0f) },
            { ParamToY, ParameterDefinition.Create<float>(0f) },
            { ParamToZ, ParameterDefinition.Create<float>(0f) },
            { ParamDuration, ParameterDefinition.Create<float>(1f) }
        };

        private readonly ITweenManager _tweenManager;

        public ObjectMoveActionStrategy(ITweenManager tweenManager)
        {
            _tweenManager = tweenManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Transform target = (Transform)command.Parameters[ParamTarget];
            if (target != null)
            {
                float toX = (float)command.Parameters[ParamToX];
                float toY = (float)command.Parameters[ParamToY];
                float toZ = (float)command.Parameters[ParamToZ];
                float duration = (float)command.Parameters[ParamDuration];

                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={target.name}, to=({toX}, {toY}, {toZ}), duration={duration}]");
                _tweenManager.MoveTo(target, new Vector3(toX, toY, toZ), duration);
            }
            yield break;
        }
    }

    [Command("Object", "Rotate")]
    public class ObjectRotateActionStrategy : IActionStrategy
    {
        public const string CommandKey = "object_rotate";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";
        public const string ParamToX = "toX";
        public const string ParamToY = "toY";
        public const string ParamToZ = "toZ";
        public const string ParamDuration = "duration";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<Transform>() },
            { ParamToX, ParameterDefinition.Create<float>(0f) },
            { ParamToY, ParameterDefinition.Create<float>(0f) },
            { ParamToZ, ParameterDefinition.Create<float>(0f) },
            { ParamDuration, ParameterDefinition.Create<float>(1f) }
        };

        private readonly ITweenManager _tweenManager;

        public ObjectRotateActionStrategy(ITweenManager tweenManager)
        {
            _tweenManager = tweenManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Transform target = (Transform)command.Parameters[ParamTarget];

            if (target != null)
            {
                float toX = (float)command.Parameters[ParamToX];
                float toY = (float)command.Parameters[ParamToY];
                float toZ = (float)command.Parameters[ParamToZ];
                float duration = (float)command.Parameters[ParamDuration];

                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={target.name}, to=({toX}, {toY}, {toZ}), duration={duration}]");
                _tweenManager.RotateTo(target, new Vector3(toX, toY, toZ), duration);
            }
            yield break;
        }
    }

    [Command("Object", "Scale")]
    public class ObjectScaleActionStrategy : IActionStrategy
    {
        public const string CommandKey = "object_scale";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";
        public const string ParamToX = "toX";
        public const string ParamToY = "toY";
        public const string ParamToZ = "toZ";
        public const string ParamDuration = "duration";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<Transform>() },
            { ParamToX, ParameterDefinition.Create<float>(1f) },
            { ParamToY, ParameterDefinition.Create<float>(1f) },
            { ParamToZ, ParameterDefinition.Create<float>(1f) },
            { ParamDuration, ParameterDefinition.Create<float>(1f) }
        };

        private readonly ITweenManager _tweenManager;

        public ObjectScaleActionStrategy(ITweenManager tweenManager)
        {
            _tweenManager = tweenManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Transform target = (Transform)command.Parameters[ParamTarget];
            if (target != null)
            {
                float toX = (float)command.Parameters[ParamToX];
                float toY = (float)command.Parameters[ParamToY];
                float toZ = (float)command.Parameters[ParamToZ];
                float duration = (float)command.Parameters[ParamDuration];

                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={target.name}, to=({toX}, {toY}, {toZ}), duration={duration}]");
                _tweenManager.ScaleTo(target, new Vector3(toX, toY, toZ), duration);
            }
            yield break;
        }
    }

    [Command("Object", "Fade")]
    public class ObjectFadeActionStrategy : IActionStrategy
    {
        public const string CommandKey = "object_fade";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTargets = "targets";
        public const string ParamFade = "fade";
        public const string ParamDuration = "duration";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTargets, ParameterDefinition.CreateObjectArray<GameObject>() },
            { ParamFade, ParameterDefinition.Create<FadeMode>(FadeMode.FadeIn) },
            { ParamDuration, ParameterDefinition.Create<float>(1f) }
        };

        private readonly IFadeController _fadeController;

        public ObjectFadeActionStrategy(IFadeController fadeController)
        {
            _fadeController = fadeController;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            if (_fadeController == null) yield break;

            List<GameObject> targetGos = new List<GameObject>();
            if (command.Parameters.TryGetValue(ParamTargets, out object targetsObj) && targetsObj != null)
            {
                if (targetsObj is IEnumerable enumerable)
                {
                    foreach (object item in enumerable)
                    {
                        if (item is GameObject itemGo) targetGos.Add(itemGo);
                    }
                }
            }

            if (targetGos.Count == 0) yield break;

            FadeMode fadeMode = (FadeMode)command.Parameters[ParamFade];
            float duration = (float)command.Parameters[ParamDuration];

            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][targets={string.Join(", ", targetGos.Select(g => g.name))}, fadeMode={fadeMode}, duration={duration}]");
            _fadeController.SetTargetObjects(targetGos.ToArray());
            _fadeController.duration = duration;

            if (fadeMode == FadeMode.FadeIn) _fadeController.StartFadeIn();
            else _fadeController.StartFadeOut();

            yield return new WaitForSeconds(duration);
        }
    }

    [Command("Object", "Hidden")]
    public class ObjectHiddenActionStrategy : IActionStrategy
    {
        public const string CommandKey = "object_hidden";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<GameObject>() }
        };

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            var targetGo = (GameObject)command.Parameters[ParamTarget];
            if (targetGo != null)
            {
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={targetGo.name}]");
                targetGo.SetActive(false);
            }
            yield break;
        }
    }

    [Command("Object", "Show")]
    public class ObjectShowActionStrategy : IActionStrategy
    {
        public const string CommandKey = "object_show";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<GameObject>() }
        };

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            var targetGo = (GameObject)command.Parameters[ParamTarget];
            if (targetGo != null)
            {
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={targetGo.name}]");
                targetGo.SetActive(true);
            }
            yield break;
        }
    }

    [Command("Object", "Click")]
    public class ObjectClickActionStrategy : IActionStrategy
    {
        public const string CommandKey = "object_click";
        string IActionStrategy.CommandKey => CommandKey;
        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>();

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Debug.Log($"{context.LogPrefix}[ObjectClick] Executing command: {command.CommandType}");
            yield break;
        }
    }

    [Command("Object", "Lock Transform")]
    public class ObjectLockTransformActionStrategy : IActionStrategy
    {
        public const string CommandKey = "object_lock_transform";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<Transform>() }
        };

        private readonly IObjectInteractionService _interactionService;

        public ObjectLockTransformActionStrategy(IObjectInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Transform target = (Transform)command.Parameters[ParamTarget];
            if (target != null)
            {
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={target.name}]");
                _interactionService.StartOperation(target);
            }
            yield break;
        }
    }

    [Command("Object", "Unlock Transform")]
    public class ObjectUnlockTransformActionStrategy : IActionStrategy
    {
        public const string CommandKey = "object_unlock_transform";
        string IActionStrategy.CommandKey => CommandKey;
        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>();

        private readonly IObjectInteractionService _interactionService;

        public ObjectUnlockTransformActionStrategy(IObjectInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}]");
            _interactionService.EndOperation();
            yield break;
        }
    }

    [Command("Object", "Apply Transform")]
    public class ObjectApplyTransformActionStrategy : IActionStrategy
    {
        public const string CommandKey = "object_apply_transform";
        string IActionStrategy.CommandKey => CommandKey;
        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>();

        private readonly IObjectInteractionService _interactionService;

        public ObjectApplyTransformActionStrategy(IObjectInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}]");
            _interactionService.SaveOperation();
            yield break;
        }
    }
}
