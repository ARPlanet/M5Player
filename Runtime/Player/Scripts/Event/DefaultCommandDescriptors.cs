using System;
using Module5.DI;
namespace Module5.Player
{
    public static class DefaultCommandDescriptors
    {
        public static ICommandDescriptor[] All(IObjectResolver container)
        {
            return new ICommandDescriptor[]
            {
                new NoneCommands.NoneCommandDescriptor(),

                new InteractionCommands.OpenCommandDescriptor(new InteractionOpenActionStrategy(TryResolve<IProjectManager>(container))),
                new InteractionCommands.CloseCommandDescriptor(new InteractionCloseActionStrategy(TryResolve<IProjectManager>(container))),

                new SceneCommands.FadeCommandDescriptor(new SceneFadeActionStrategy(
                    TryResolve<ISceneManager>(container),
                    TryResolve<IFadeController>(container)
                )),
                new SceneCommands.ChangeCommandDescriptor(new SceneChangeActionStrategy(
                    TryResolve<ISceneManager>(container),
                    TryResolve<IAssetLoaderManager>(container)
                )),
                new SceneCommands.PauseCommandDescriptor(new ScenePauseActionStrategy()),
                new SceneCommands.ResumeCommandDescriptor(new SceneResumeActionStrategy()),

                new CameraCommands.TakePhotoCommandDescriptor(new CameraTakePhotoActionStrategy()),
                new CameraCommands.RecordVideoCommandDescriptor(new CameraRecordVideoActionStrategy()),
                new CameraCommands.FlipFrontCameraCommandDescriptor(new CameraFlipFrontCameraActionStrategy()),
                new CameraCommands.PreviewSaveCommandDescriptor(new CameraPreviewSaveActionStrategy()),
                new CameraCommands.PreviewUploadCommandDescriptor(new CameraPreviewUploadActionStrategy()),
                new CameraCommands.PreviewShareCommandDescriptor(new CameraPreviewShareActionStrategy()),

                new AnchorCommands.AttachToTargetCommandDescriptor(new AnchorAttachToTargetActionStrategy()),
                new AnchorCommands.DetachFromTargetCommandDescriptor(new AnchorDetachFromTargetActionStrategy()),

                new EventHandlerCommands.EnableCommandDescriptor(new EventHandlerEnableActionStrategy()),
                new EventHandlerCommands.DisableCommandDescriptor(new EventHandlerDisableActionStrategy()),

                new ObjectCommands.MoveCommandDescriptor(new ObjectMoveActionStrategy(TryResolve<ITweenManager>(container))),
                new ObjectCommands.RotateCommandDescriptor(new ObjectRotateActionStrategy(TryResolve<ITweenManager>(container))),
                new ObjectCommands.ScaleCommandDescriptor(new ObjectScaleActionStrategy(TryResolve<ITweenManager>(container))),
                new ObjectCommands.FadeCommandDescriptor(new ObjectFadeActionStrategy(TryResolve<IFadeController>(container))),
                new ObjectCommands.HiddenCommandDescriptor(new ObjectHiddenActionStrategy()),
                new ObjectCommands.ShowCommandDescriptor(new ObjectShowActionStrategy()),
                new ObjectCommands.ClickCommandDescriptor(new ObjectClickActionStrategy()),
                new ObjectCommands.LockTransformCommandDescriptor(new ObjectLockTransformActionStrategy(TryResolve<IObjectInteractionService>(container))),
                new ObjectCommands.UnlockTransformCommandDescriptor(new ObjectUnlockTransformActionStrategy(TryResolve<IObjectInteractionService>(container))),
                new ObjectCommands.ApplyTransformCommandDescriptor(new ObjectApplyTransformActionStrategy(TryResolve<IObjectInteractionService>(container))),

                new ComponentCommands.ChangeTextCommandDescriptor(new ComponentChangeTextActionStrategy()),
                new ComponentCommands.ChangeImageCommandDescriptor(new ComponentChangeImageActionStrategy(TryResolve<IAssetLoaderManager>(container))),
                new ComponentCommands.TextInputCommandDescriptor(new ComponentTextInputActionStrategy(TryResolve<IVariableManager>(container))),

                new AudioCommands.PlayCommandDescriptor(new AudioPlayActionStrategy()),
                new AudioCommands.PauseCommandDescriptor(new AudioPauseActionStrategy()),
                new AudioCommands.StopCommandDescriptor(new AudioStopActionStrategy()),

                new AnimationCommands.PlayCommandDescriptor(new AnimationPlayActionStrategy()),
                new AnimationCommands.PauseCommandDescriptor(new AnimationPauseActionStrategy()),
                new AnimationCommands.StopCommandDescriptor(new AnimationStopActionStrategy()),

                new VideoCommands.PlayCommandDescriptor(new VideoPlayActionStrategy()),
                new VideoCommands.PauseCommandDescriptor(new VideoPauseActionStrategy()),
                new VideoCommands.StopCommandDescriptor(new VideoStopActionStrategy()),

                new GlobalValueCommands.SetCommandDescriptor(new GlobalValueSetActionStrategy(TryResolve<IVariableManager>(container))),
                new GlobalValueCommands.SetGlobalVariableCommandDescriptor(new SetGlobalVariableActionStrategy(TryResolve<IVariableManager>(container))),

                new TimerCommands.StartCommandDescriptor(new TimerStartActionStrategy(TryResolve<ITimerManager>(container))),
                new TimerCommands.StopCommandDescriptor(new TimerStopActionStrategy(TryResolve<ITimerManager>(container))),
                new TimerCommands.AddCommandDescriptor(new TimerAddActionStrategy(TryResolve<ITimerManager>(container))),
                new TimerCommands.SubCommandDescriptor(new TimerSubActionStrategy(TryResolve<ITimerManager>(container))),

                new RemoteFunctionCommands.CallCommandDescriptor(new RemoteFunctionCallActionStrategy(TryResolve<IVariableManager>(container))),
                new NativeFunctionCommands.CallCommandDescriptor(new NativeFunctionCallActionStrategy()),
                new UriCommands.OpenCommandDescriptor(new UriOpenActionStrategy()),
                new MiscCommands.RandExecCommandDescriptor(new RandExecActionStrategy()),
                new MiscCommands.DelayExecCommandDescriptor(new DelayExecActionStrategy()),
                new MiscCommands.LogCommandDescriptor(new LogActionStrategy(TryResolve<IVariableManager>(container)))
            };
        }

        private static T TryResolve<T>(IObjectResolver container) where T : class
        {
            if (container == null) return null;
            return container.TryResolve<T>(out var result) ? result : null;
        }
    }
}
