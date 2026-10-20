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

    [Command("Scene", "Fade")]
    public class SceneFadeActionStrategy : IActionStrategy
    {
        public const string CommandKey = "scene_fade";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamFade = "fade";
        public const string ParamDuration = "duration";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamFade, ParameterDefinition.Create<FadeMode>(FadeMode.FadeIn) },
            { ParamDuration, ParameterDefinition.Create<float>(1f) }
        };

        private readonly ISceneManager _sceneManager;
        private readonly IFadeController _fadeController;

        public SceneFadeActionStrategy(ISceneManager sceneManager, IFadeController fadeController)
        {
            _sceneManager = sceneManager;
            _fadeController = fadeController;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            FadeMode fadeMode = (FadeMode)command.Parameters[ParamFade];
            float duration = (float)command.Parameters[ParamDuration];

            var root = _sceneManager.Root; // Scenes
            List<GameObject> targetObjects = new List<GameObject>();
            if (root != null)
            {
                for (int i = 0; i < root.childCount; i++)
                {
                    Transform sceneRoot = root.GetChild(i); // Scene Root
                    if (!sceneRoot.gameObject.activeSelf) continue;
                    for (int j = 0; j < sceneRoot.childCount; j++)
                    {
                        Transform child = sceneRoot.GetChild(j);
                        if (!child.gameObject.activeSelf) continue;
                        targetObjects.Add(child.gameObject);
                    }
                }
            }

            if (_fadeController != null)
            {
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][fadeMode={fadeMode}, duration={duration}]");
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

    [Command("Scene", "Change")]
    public class SceneChangeActionStrategy : IActionStrategy
    {
        public const string CommandKey = "scene_change";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamScene = "scene";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamScene, ParameterDefinition.CreateObject<SceneData>() }
        };

        private readonly ISceneManager _sceneManager;
        private readonly IAssetLoaderManager _assetLoaderManager;

        public SceneChangeActionStrategy(ISceneManager sceneManager, IAssetLoaderManager assetLoaderManager)
        {
            _sceneManager = sceneManager;
            _assetLoaderManager = assetLoaderManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            if (command.Parameters.TryGetValue(ParamScene, out object sceneObj) && sceneObj is SceneData sceneData && _sceneManager != null)
            {
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][scene={sceneData.Name}]");
                _ = _sceneManager.OpenSceneAsync(sceneData);
            }
            yield break;
        }
    }

    [Command("Scene", "Pause")]
    public class ScenePauseActionStrategy : IActionStrategy
    {
        public const string CommandKey = "scene_pause";
        string IActionStrategy.CommandKey => CommandKey;
        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>();

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}]");
            yield break;
        }
    }

    [Command("Scene", "Resume")]
    public class SceneResumeActionStrategy : IActionStrategy
    {
        public const string CommandKey = "scene_resume";
        string IActionStrategy.CommandKey => CommandKey;
        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>();

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}]");
            yield break;
        }
    }
}
