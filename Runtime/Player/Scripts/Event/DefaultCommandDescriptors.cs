using System;
using Module5.DI;

namespace Module5.Player
{
    public static class DefaultCommandDescriptors
    {
        public static IActionStrategy[] All(IObjectResolver container)
        {
            return new IActionStrategy[]
            {
                new InteractionOpenActionStrategy(TryResolve<IProjectManager>(container)),
                new InteractionCloseActionStrategy(TryResolve<IProjectManager>(container)),

                new SceneFadeActionStrategy(
                    TryResolve<ISceneManager>(container),
                    TryResolve<IFadeController>(container)
                ),
                new SceneChangeActionStrategy(
                    TryResolve<ISceneManager>(container),
                    TryResolve<IAssetLoaderManager>(container)
                ),
                new ScenePauseActionStrategy(),
                new SceneResumeActionStrategy(),

                new CameraTakePhotoActionStrategy(),
                new CameraRecordVideoActionStrategy(),
                new CameraFlipFrontCameraActionStrategy(),
                new CameraPreviewSaveActionStrategy(),
                new CameraPreviewUploadActionStrategy(),
                new CameraPreviewShareActionStrategy(),

                new AnchorAttachToTargetActionStrategy(),
                new AnchorDetachFromTargetActionStrategy(),

                new EventHandlerEnableActionStrategy(),
                new EventHandlerDisableActionStrategy(),

                new ObjectMoveActionStrategy(TryResolve<ITweenManager>(container)),
                new ObjectRotateActionStrategy(TryResolve<ITweenManager>(container)),
                new ObjectScaleActionStrategy(TryResolve<ITweenManager>(container)),
                new ObjectFadeActionStrategy(TryResolve<IFadeController>(container)),
                new ObjectHiddenActionStrategy(),
                new ObjectShowActionStrategy(),
                new ObjectClickActionStrategy(),
                new ObjectLockTransformActionStrategy(TryResolve<IObjectInteractionService>(container)),
                new ObjectUnlockTransformActionStrategy(TryResolve<IObjectInteractionService>(container)),
                new ObjectApplyTransformActionStrategy(TryResolve<IObjectInteractionService>(container)),

                new ComponentChangeTextActionStrategy(),
                new ComponentChangeImageActionStrategy(TryResolve<IAssetLoaderManager>(container)),
                new ComponentTextInputActionStrategy(TryResolve<IVariableManager>(container)),

                new AudioPlayActionStrategy(),
                new AudioPauseActionStrategy(),
                new AudioStopActionStrategy(),

                new BgmPlayActionStrategy(TryResolve<IBgmManager>(container), TryResolve<IAssetLoaderManager>(container)),
                new BgmPauseActionStrategy(TryResolve<IBgmManager>(container)),
                new BgmStopActionStrategy(TryResolve<IBgmManager>(container)),
                new BgmSetVolumeActionStrategy(TryResolve<IBgmManager>(container)),

                new AnimationPlayActionStrategy(),
                new AnimationPauseActionStrategy(),
                new AnimationStopActionStrategy(),

                new VideoPlayActionStrategy(),
                new VideoPauseActionStrategy(),
                new VideoStopActionStrategy(),

                new GlobalValueSetActionStrategy(TryResolve<IVariableManager>(container)),
                new GlobalValueNumericAddActionStrategy(TryResolve<IVariableManager>(container)),
                new GlobalValueNumericSubtractActionStrategy(TryResolve<IVariableManager>(container)),

                new TimerStartActionStrategy(TryResolve<ITimerManager>(container)),
                new TimerStopActionStrategy(TryResolve<ITimerManager>(container)),
                new TimerAddActionStrategy(TryResolve<ITimerManager>(container)),
                new TimerSubActionStrategy(TryResolve<ITimerManager>(container)),

                new RemoteFunctionCallActionStrategy(TryResolve<IVariableManager>(container)),
                new NativeFunctionCallActionStrategy(),
                new UriOpenActionStrategy(),
                new RandExecActionStrategy(),
                new DelayExecActionStrategy(),
                new LogActionStrategy(TryResolve<IVariableManager>(container))
            };
        }

        private static T TryResolve<T>(IObjectResolver container) where T : class
        {
            if (container == null) return null;
            return container.TryResolve<T>(out var result) ? result : null;
        }
    }
}
