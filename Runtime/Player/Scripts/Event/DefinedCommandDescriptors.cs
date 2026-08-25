using System;
using System.Collections.Generic;

namespace Module5.Player
{
    public static class NoneCommands
    {
        public const string KeyNone = "None";

        [Command("None", "None", "None")]
        public class NoneCommandDescriptor : CommandDescriptor
        {
            public NoneCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyNone, actionStrategy) { }
        }
    }

    public static class InteractionCommands
    {
        public const string KeyOpen = "InteractionOpen";
        public const string KeyClose = "InteractionClose";

        [Command("Interaction Open", "Interaction", "Open")]
        public class OpenCommandDescriptor : CommandDescriptor
        {
            public OpenCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyOpen, actionStrategy) { }
        }

        [Command("Interaction Close", "Interaction", "Close")]
        public class CloseCommandDescriptor : CommandDescriptor
        {
            public CloseCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyClose, actionStrategy) { }
        }
    }

    public static class SceneCommands
    {
        public const string KeyFade = "SceneFade";
        public const string KeyChange = "SceneChange";
        public const string KeyPause = "ScenePause";
        public const string KeyResume = "SceneResume";

        [Command("Scene Fade", "Scene", "Fade")]
        public class FadeCommandDescriptor : CommandDescriptor
        {
            public FadeCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyFade, actionStrategy) { }
        }

        [Command("Scene Change", "Scene", "Change")]
        public class ChangeCommandDescriptor : CommandDescriptor
        {
            public ChangeCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyChange, actionStrategy) { }
        }

        [Command("Scene Pause", "Scene", "Pause")]
        public class PauseCommandDescriptor : CommandDescriptor
        {
            public PauseCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyPause, actionStrategy) { }
        }

        [Command("Scene Resume", "Scene", "Resume")]
        public class ResumeCommandDescriptor : CommandDescriptor
        {
            public ResumeCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyResume, actionStrategy) { }
        }
    }

    public static class CameraCommands
    {
        public const string KeyTakePhoto = "CameraTakePhoto";
        public const string KeyRecordVideo = "CameraRecordVideo";
        public const string KeyFlipFrontCamera = "CameraFlipFrontCamera";
        public const string KeyPreviewSave = "CameraPreviewSave";
        public const string KeyPreviewUpload = "CameraPreviewUpload";
        public const string KeyPreviewShare = "CameraPreviewShare";

        [Command("Camera Take Photo", "Camera", "TakePhoto")]
        public class TakePhotoCommandDescriptor : CommandDescriptor
        {
            public TakePhotoCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyTakePhoto, actionStrategy) { }
        }

        [Command("Camera Record Video", "Camera", "RecordVideo")]
        public class RecordVideoCommandDescriptor : CommandDescriptor
        {
            public RecordVideoCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyRecordVideo, actionStrategy) { }
        }

        [Command("Camera Flip Front Camera", "Camera", "FlipFrontCamera")]
        public class FlipFrontCameraCommandDescriptor : CommandDescriptor
        {
            public FlipFrontCameraCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyFlipFrontCamera, actionStrategy) { }
        }

        [Command("Camera Preview Save", "Camera", "PreviewSave")]
        public class PreviewSaveCommandDescriptor : CommandDescriptor
        {
            public PreviewSaveCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyPreviewSave, actionStrategy) { }
        }

        [Command("Camera Preview Upload", "Camera", "PreviewUpload")]
        public class PreviewUploadCommandDescriptor : CommandDescriptor
        {
            public PreviewUploadCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyPreviewUpload, actionStrategy) { }
        }

        [Command("Camera Preview Share", "Camera", "PreviewShare")]
        public class PreviewShareCommandDescriptor : CommandDescriptor
        {
            public PreviewShareCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyPreviewShare, actionStrategy) { }
        }
    }

    public static class AnchorCommands
    {
        public const string KeyAttachToTarget = "AnchorAttachToTarget";
        public const string KeyDetachFromTarget = "AnchorDetachFromTarget";

        [Command("Anchor Attach To Target", "Anchor", "AttachToTarget")]
        public class AttachToTargetCommandDescriptor : CommandDescriptor
        {
            public AttachToTargetCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyAttachToTarget, actionStrategy) { }
        }

        [Command("Anchor Detach From Target", "Anchor", "DetachFromTarget")]
        public class DetachFromTargetCommandDescriptor : CommandDescriptor
        {
            public DetachFromTargetCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyDetachFromTarget, actionStrategy) { }
        }
    }

    public static class EventHandlerCommands
    {
        public const string KeyEnable = "EventHandlerEnable";
        public const string KeyDisable = "EventHandlerDisable";

        [Command("EventHandler Enable", "EventHandler", "Enable")]
        public class EnableCommandDescriptor : CommandDescriptor
        {
            public EnableCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyEnable, actionStrategy) { }
        }

        [Command("EventHandler Disable", "EventHandler", "Disable")]
        public class DisableCommandDescriptor : CommandDescriptor
        {
            public DisableCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyDisable, actionStrategy) { }
        }
    }

    public static class ObjectCommands
    {
        public const string KeyMove = "ObjectMove";
        public const string KeyRotate = "ObjectRotate";
        public const string KeyScale = "ObjectScale";
        public const string KeyFade = "ObjectFade";
        public const string KeyHidden = "ObjectHidden";
        public const string KeyShow = "ObjectShow";
        public const string KeyClick = "ObjectClick";
        public const string KeyLockTransform = "ObjectLockTransform";
        public const string KeyUnlockTransform = "ObjectUnlockTransform";
        public const string KeyApplyTransform = "ObjectApplyTransform";

        [Command("Object Move", "Object", "Move")]
        public class MoveCommandDescriptor : CommandDescriptor
        {
            public MoveCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyMove, actionStrategy) { }
        }

        [Command("Object Rotate", "Object", "Rotate")]
        public class RotateCommandDescriptor : CommandDescriptor
        {
            public RotateCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyRotate, actionStrategy) { }
        }

        [Command("Object Scale", "Object", "Scale")]
        public class ScaleCommandDescriptor : CommandDescriptor
        {
            public ScaleCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyScale, actionStrategy) { }
        }

        [Command("Object Fade", "Object", "Fade")]
        public class FadeCommandDescriptor : CommandDescriptor
        {
            public FadeCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyFade, actionStrategy) { }
        }

        [Command("Object Hidden", "Object", "Hidden")]
        public class HiddenCommandDescriptor : CommandDescriptor
        {
            public HiddenCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyHidden, actionStrategy) { }
        }

        [Command("Object Show", "Object", "Show")]
        public class ShowCommandDescriptor : CommandDescriptor
        {
            public ShowCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyShow, actionStrategy) { }
        }

        [Command("Object Click", "Object", "Click")]
        public class ClickCommandDescriptor : CommandDescriptor
        {
            public ClickCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyClick, actionStrategy) { }
        }

        [Command("Object Lock Transform", "Object", "LockTransform")]
        public class LockTransformCommandDescriptor : CommandDescriptor
        {
            public LockTransformCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyLockTransform, actionStrategy) { }
        }

        [Command("Object Unlock Transform", "Object", "UnlockTransform")]
        public class UnlockTransformCommandDescriptor : CommandDescriptor
        {
            public UnlockTransformCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyUnlockTransform, actionStrategy) { }
        }

        [Command("Object Apply Transform", "Object", "ApplyTransform")]
        public class ApplyTransformCommandDescriptor : CommandDescriptor
        {
            public ApplyTransformCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyApplyTransform, actionStrategy) { }
        }
    }

    public static class ComponentCommands
    {
        public const string KeyChangeText = "ComponentChangeText";
        public const string KeyChangeImage = "ComponentChangeImage";
        public const string KeyTextInput = "ComponentTextInput";

        [Command("Component Change Text", "Component", "ChangeText")]
        public class ChangeTextCommandDescriptor : CommandDescriptor
        {
            public ChangeTextCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyChangeText, actionStrategy) { }
        }

        [Command("Component Change Image", "Component", "ChangeImage")]
        public class ChangeImageCommandDescriptor : CommandDescriptor
        {
            public ChangeImageCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyChangeImage, actionStrategy) { }
        }

        [Command("Component Text Input", "Component", "TextInput")]
        public class TextInputCommandDescriptor : CommandDescriptor
        {
            public TextInputCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyTextInput, actionStrategy) { }
        }
    }

    public static class AudioCommands
    {
        public const string KeyPlay = "AudioPlay";
        public const string KeyPause = "AudioPause";
        public const string KeyStop = "AudioStop";

        [Command("Audio Play", "Audio", "Play")]
        public class PlayCommandDescriptor : CommandDescriptor
        {
            public PlayCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyPlay, actionStrategy) { }
        }

        [Command("Audio Pause", "Audio", "Pause")]
        public class PauseCommandDescriptor : CommandDescriptor
        {
            public PauseCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyPause, actionStrategy) { }
        }

        [Command("Audio Stop", "Audio", "Stop")]
        public class StopCommandDescriptor : CommandDescriptor
        {
            public StopCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyStop, actionStrategy) { }
        }
    }

    public static class AnimationCommands
    {
        public const string KeyPlay = "AnimationPlay";
        public const string KeyPause = "AnimationPause";
        public const string KeyStop = "AnimationStop";

        [Command("Animation Play", "Animation", "Play")]
        public class PlayCommandDescriptor : CommandDescriptor
        {
            public PlayCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyPlay, actionStrategy) { }
        }

        [Command("Animation Pause", "Animation", "Pause")]
        public class PauseCommandDescriptor : CommandDescriptor
        {
            public PauseCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyPause, actionStrategy) { }
        }

        [Command("Animation Stop", "Animation", "Stop")]
        public class StopCommandDescriptor : CommandDescriptor
        {
            public StopCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyStop, actionStrategy) { }
        }
    }

    public static class VideoCommands
    {
        public const string KeyPlay = "VideoPlay";
        public const string KeyPause = "VideoPause";
        public const string KeyStop = "VideoStop";

        [Command("Video Play", "Video", "Play")]
        public class PlayCommandDescriptor : CommandDescriptor
        {
            public PlayCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyPlay, actionStrategy) { }
        }

        [Command("Video Pause", "Video", "Pause")]
        public class PauseCommandDescriptor : CommandDescriptor
        {
            public PauseCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyPause, actionStrategy) { }
        }

        [Command("Video Stop", "Video", "Stop")]
        public class StopCommandDescriptor : CommandDescriptor
        {
            public StopCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyStop, actionStrategy) { }
        }
    }

    public static class GlobalValueCommands
    {
        public const string KeySet = "GlobalValueSet";
        public const string KeySetGlobalVariable = "SetGlobalVariable";

        [Command("Global Value Set", "GlobalValue", "Set")]
        public class SetCommandDescriptor : CommandDescriptor
        {
            public SetCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeySet, actionStrategy) { }
        }

        [Command("Set Global Variable", "Legacy", "SetGlobalVariable")]
        public class SetGlobalVariableCommandDescriptor : CommandDescriptor
        {
            public SetGlobalVariableCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeySetGlobalVariable, actionStrategy) { }
        }
    }

    public static class TimerCommands
    {
        public const string KeyStart = "TimerStart";
        public const string KeyStop = "TimerStop";
        public const string KeyAdd = "TimerAdd";
        public const string KeySub = "TimerSub";

        [Command("Timer Start", "Timer", "Start")]
        public class StartCommandDescriptor : CommandDescriptor
        {
            public StartCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyStart, actionStrategy) { }
        }

        [Command("Timer Stop", "Timer", "Stop")]
        public class StopCommandDescriptor : CommandDescriptor
        {
            public StopCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyStop, actionStrategy) { }
        }

        [Command("Timer Add", "Timer", "Add")]
        public class AddCommandDescriptor : CommandDescriptor
        {
            public AddCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyAdd, actionStrategy) { }
        }

        [Command("Timer Sub", "Timer", "Sub")]
        public class SubCommandDescriptor : CommandDescriptor
        {
            public SubCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeySub, actionStrategy) { }
        }
    }

    public static class RemoteFunctionCommands
    {
        public const string KeyCall = "RemoteFunctionCall";

        [Command("Remote Function Call", "RemoteFunction", "Call")]
        public class CallCommandDescriptor : CommandDescriptor
        {
            public CallCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyCall, actionStrategy) { }
        }
    }

    public static class NativeFunctionCommands
    {
        public const string KeyCall = "NativeFunctionCall";

        [Command("Native Function Call", "NativeFunction", "Call")]
        public class CallCommandDescriptor : CommandDescriptor
        {
            public CallCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyCall, actionStrategy) { }
        }
    }

    public static class UriCommands
    {
        public const string KeyOpen = "UriOpen";

        [Command("Uri Open", "Uri", "Open")]
        public class OpenCommandDescriptor : CommandDescriptor
        {
            public OpenCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyOpen, actionStrategy) { }
        }
    }

    public static class MiscCommands
    {
        public const string KeyRandExec = "RandExec";
        public const string KeyDelayExec = "DelayExec";
        public const string KeyLog = "Log";

        [Command("Rand Exec", "Misc", "RandExec")]
        public class RandExecCommandDescriptor : CommandDescriptor
        {
            public RandExecCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyRandExec, actionStrategy) { }
        }

        [Command("Delay Exec", "Misc", "DelayExec")]
        public class DelayExecCommandDescriptor : CommandDescriptor
        {
            public DelayExecCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyDelayExec, actionStrategy) { }
        }

        [Command("Log", "Legacy", "Log")]
        public class LogCommandDescriptor : CommandDescriptor
        {
            public LogCommandDescriptor(IActionStrategy actionStrategy = null)
                : base(KeyLog, actionStrategy) { }
        }
    }
}
