using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Module5.Player
{
    public class ObjectMoveActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";
        public const string ParamToX = "toX";
        public const string ParamToY = "toY";
        public const string ParamToZ = "toZ";
        public const string ParamDuration = "duration";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() },
            { ParamToX, 0f },
            { ParamToY, 0f },
            { ParamToZ, 0f },
            { ParamDuration, 1f }
        };

        private readonly ITweenManager _tweenManager;

        public ObjectMoveActionStrategy(ITweenManager tweenManager)
        {
            _tweenManager = tweenManager;
        }

        public IEnumerator Execute(M5Command command)
        {
            var target = command.Parameters[ParamTarget];
            Transform targetTransform = target is GameObject go ? go.transform : (target as Transform);
            if (targetTransform != null)
            {
                float toX = (float)command.Parameters[ParamToX];
                float toY = (float)command.Parameters[ParamToY];
                float toZ = (float)command.Parameters[ParamToZ];
                float duration = (float)command.Parameters[ParamDuration];

                Debug.Log($"[Action {command.CommandType}][target={targetTransform.name}, to=({toX}, {toY}, {toZ}), duration={duration}]");
                _tweenManager.MoveTo(targetTransform, new Vector3(toX, toY, toZ), duration);
            }
            yield break;
        }
    }

    public class ObjectRotateActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";
        public const string ParamToX = "toX";
        public const string ParamToY = "toY";
        public const string ParamToZ = "toZ";
        public const string ParamDuration = "duration";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() },
            { ParamToX, 0f },
            { ParamToY, 0f },
            { ParamToZ, 0f },
            { ParamDuration, 1f }
        };

        private readonly ITweenManager _tweenManager;

        public ObjectRotateActionStrategy(ITweenManager tweenManager)
        {
            _tweenManager = tweenManager;
        }

        public IEnumerator Execute(M5Command command)
        {
            var target = command.Parameters[ParamTarget];
            Transform targetTransform = target is GameObject go ? go.transform : (target as Transform);
            if (targetTransform != null)
            {
                float toX = (float)command.Parameters[ParamToX];
                float toY = (float)command.Parameters[ParamToY];
                float toZ = (float)command.Parameters[ParamToZ];
                float duration = (float)command.Parameters[ParamDuration];

                Debug.Log($"[Action {command.CommandType}][target={targetTransform.name}, to=({toX}, {toY}, {toZ}), duration={duration}]");
                _tweenManager.RotateTo(targetTransform, new Vector3(toX, toY, toZ), duration);
            }
            yield break;
        }
    }

    public class ObjectScaleActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";
        public const string ParamToX = "toX";
        public const string ParamToY = "toY";
        public const string ParamToZ = "toZ";
        public const string ParamDuration = "duration";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() },
            { ParamToX, 1f },
            { ParamToY, 1f },
            { ParamToZ, 1f },
            { ParamDuration, 1f }
        };

        private readonly ITweenManager _tweenManager;

        public ObjectScaleActionStrategy(ITweenManager tweenManager)
        {
            _tweenManager = tweenManager;
        }

        public IEnumerator Execute(M5Command command)
        {
            var target = command.Parameters[ParamTarget];
            Transform targetTransform = target is GameObject go ? go.transform : (target as Transform);
            if (targetTransform != null)
            {
                float toX = (float)command.Parameters[ParamToX];
                float toY = (float)command.Parameters[ParamToY];
                float toZ = (float)command.Parameters[ParamToZ];
                float duration = (float)command.Parameters[ParamDuration];

                Debug.Log($"[Action {command.CommandType}][target={targetTransform.name}, to=({toX}, {toY}, {toZ}), duration={duration}]");
                _tweenManager.ScaleTo(targetTransform, new Vector3(toX, toY, toZ), duration);
            }
            yield break;
        }
    }

    public class ObjectFadeActionStrategy : IActionStrategy
    {
        public const string ParamTargets = "targets";
        public const string ParamFade = "fade";
        public const string ParamDuration = "duration";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTargets, new string[0] },
            { ParamFade, FadeMode.FadeIn },
            { ParamDuration, 1f }
        };

        private readonly IFadeController _fadeController;

        public ObjectFadeActionStrategy(IFadeController fadeController)
        {
            _fadeController = fadeController;
        }

        public IEnumerator Execute(M5Command command)
        {
            if (_fadeController == null) yield break;

            List<GameObject> targetGos = new List<GameObject>();
            if (command.Parameters.TryGetValue(ParamTargets, out object targetsObj) && targetsObj != null)
            {
                if (targetsObj is IEnumerable enumerable && targetsObj is not string)
                {
                    foreach (object item in enumerable)
                    {
                        if (item is GameObject itemGo) targetGos.Add(itemGo);
                        else if (item is Component itemComp) targetGos.Add(itemComp.gameObject);
                    }
                }
                else if (targetsObj is GameObject go) targetGos.Add(go);
                else if (targetsObj is Component comp) targetGos.Add(comp.gameObject);
            }

            if (targetGos.Count == 0) yield break;

            FadeMode fadeMode = (FadeMode)command.Parameters[ParamFade];
            float duration = (float)command.Parameters[ParamDuration];

            Debug.Log($"[Action {command.CommandType}][targets={string.Join(", ", targetGos.Select(g => g.name))}, fadeMode={fadeMode}, duration={duration}]");
            _fadeController.SetTargetObjects(targetGos.ToArray());
            _fadeController.duration = duration;

            if (fadeMode == FadeMode.FadeIn) _fadeController.StartFadeIn();
            else _fadeController.StartFadeOut();

            yield return new WaitForSeconds(duration);
        }
    }

    public class ObjectHiddenActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() }
        };

        public IEnumerator Execute(M5Command command)
        {
            var targetGo = (GameObject)command.Parameters[ParamTarget];
            if (targetGo != null)
            {
                Debug.Log($"[Action {command.CommandType}][target={targetGo.name}]");
                targetGo.SetActive(false);
            }
            yield break;
        }
    }

    public class ObjectShowActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() }
        };

        public IEnumerator Execute(M5Command command)
        {
            var targetGo = (GameObject)command.Parameters[ParamTarget];
            if (targetGo != null)
            {
                Debug.Log($"[Action {command.CommandType}][target={targetGo.name}]");
                targetGo.SetActive(true);
            }
            yield break;
        }
    }

    public class ObjectClickActionStrategy : IActionStrategy
    {
        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>();

        public IEnumerator Execute(M5Command command)
        {
            Debug.Log($"[ObjectClick] Executing command: {command.CommandType}");
            yield break;
        }
    }

    public class ObjectLockTransformActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() }
        };

        private readonly IObjectInteractionService _interactionService;

        public ObjectLockTransformActionStrategy(IObjectInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        public IEnumerator Execute(M5Command command)
        {
            var target = command.Parameters[ParamTarget];
            Transform targetTransform = target is GameObject go ? go.transform : (target as Transform);
            if (targetTransform != null)
            {
                Debug.Log($"[Action {command.CommandType}][target={targetTransform.name}]");
                _interactionService.StartOperation(targetTransform);
            }
            yield break;
        }
    }

    public class ObjectUnlockTransformActionStrategy : IActionStrategy
    {
        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>();

        private readonly IObjectInteractionService _interactionService;

        public ObjectUnlockTransformActionStrategy(IObjectInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        public IEnumerator Execute(M5Command command)
        {
            Debug.Log($"[Action {command.CommandType}]");
            _interactionService.EndOperation();
            yield break;
        }
    }

    public class ObjectApplyTransformActionStrategy : IActionStrategy
    {
        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>();

        private readonly IObjectInteractionService _interactionService;

        public ObjectApplyTransformActionStrategy(IObjectInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        public IEnumerator Execute(M5Command command)
        {
            Debug.Log($"[Action {command.CommandType}]");
            _interactionService.SaveOperation();
            yield break;
        }
    }
}
