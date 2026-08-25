using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    public enum FadeMode
    {
        FadeIn,
        FadeOut
    }
    public interface IFadeController
    {
        void SetTargetObjects(GameObject[] objects);
        float duration { get; set; }
        void StartFadeIn();
        void StartFadeOut();
    }

    public class SceneFadeActionStrategy : IActionStrategy
    {
        public const string ParamFade = "fade";
        public const string ParamDuration = "duration";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamFade, FadeMode.FadeIn },
            { ParamDuration, 1f }
        };

        private readonly ISceneManager _sceneManager;
        private readonly IFadeController _fadeController;

        public SceneFadeActionStrategy(ISceneManager sceneManager, IFadeController fadeController)
        {
            _sceneManager = sceneManager;
            _fadeController = fadeController;
        }

        public IEnumerator Execute(M5Command command)
        {
            FadeMode fadeMode = (FadeMode)command.Parameters[ParamFade];
            float duration = (float)command.Parameters[ParamDuration];

            var root = _sceneManager.Root;
            List<GameObject> targetObjects = new List<GameObject>();
            if (root != null)
            {
                for (int i = 0; i < root.childCount; i++)
                {
                    Transform child = root.GetChild(i);
                    if (!child.gameObject.activeSelf) continue;
                    for (int j = 0; j < child.childCount; j++)
                    {
                        Transform subChild = child.GetChild(j);
                        if (!subChild.gameObject.activeSelf) continue;
                        targetObjects.Add(subChild.gameObject);
                    }
                }
            }

            if (_fadeController != null)
            {
                Debug.Log($"[Action {command.CommandType}][fadeMode={fadeMode}, duration={duration}]");
                _fadeController.SetTargetObjects(targetObjects.ToArray());
                _fadeController.duration = duration;
                if (fadeMode == FadeMode.FadeIn)
                {
                    _fadeController.StartFadeIn();
                }
                else
                {
                    _fadeController.StartFadeOut();
                }
                
                yield return new WaitForSeconds(duration);
            }
        }
    }

    public class SceneChangeActionStrategy : IActionStrategy
    {
        public const string ParamScene = "scene";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamScene, System.Guid.Empty.ToString() }
        };

        private readonly ISceneManager _sceneManager;
        private readonly IAssetLoaderManager _assetLoaderManager;

        public SceneChangeActionStrategy(ISceneManager sceneManager, IAssetLoaderManager assetLoaderManager)
        {
            _sceneManager = sceneManager;
            _assetLoaderManager = assetLoaderManager;
        }

        public IEnumerator Execute(M5Command command)
        {
            if (command.Parameters.TryGetValue(ParamScene, out object sceneObj) && sceneObj is SceneData sceneData && _sceneManager != null)
            {
                Debug.Log($"[Action {command.CommandType}][scene={sceneData.Name}]");
                var openTask = _sceneManager.OpenSceneAsync(sceneData);
                while (!openTask.IsCompleted) yield return null;
            }
            yield break;
        }
    }

    public class ScenePauseActionStrategy : IActionStrategy
    {
        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>();

        public IEnumerator Execute(M5Command command)
        {
            Debug.Log($"[Action {command.CommandType}]");
            yield break;
        }
    }

    public class SceneResumeActionStrategy : IActionStrategy
    {
        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>();

        public IEnumerator Execute(M5Command command)
        {
            Debug.Log($"[Action {command.CommandType}]");
            yield break;
        }
    }
}
