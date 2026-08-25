using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    public static class NoneEvents
    {
        public const string KeyNone = "None";
    }

    public static class AppEvents
    {
        public const string KeyPause = "OnAppPause";
        public const string KeyResume = "OnAppResume";

        public static M5Event CreatePauseEvent(object source = default)
        {
            return new M5Event(KeyPause, source);
        }

        public static M5Event CreateResumeEvent(object source = default)
        {
            return new M5Event(KeyResume, source);
        }
    }

    public static class SceneEvents
    {
        public const string KeyStart = "OnSceneStart";
        public const string KeyExit = "OnSceneExit";

        public static M5Event CreateStartEvent(object source)
        {
            return new M5Event(KeyStart, source);
        }

        public static M5Event CreateExitEvent(object source)
        {
            return new M5Event(KeyExit, source);
        }
    }

    public static class ObjectEvents
    {
        public const string KeyEnable = "OnObjectEnable";
        public const string KeyDisable = "OnObjectDisable";
        public const string KeyClick = "OnObjectClick";
        public const string KeyLongTap = "OnObjectLongTap";

        public const string ContextSource = "source";

        public static M5Event CreateEnableEvent(object source)
        {
            return new M5Event(KeyEnable, source);
        }

        public static M5Event CreateDisableEvent(object source)
        {
            return new M5Event(KeyDisable, source);
        }

        public static M5Event CreateClickEvent(object source, Click click)
        {
            return new M5Event(KeyClick, source, new Dictionary<string, object>
            {
                { ContextSource, click }
            });
        }

        public static M5Event CreateLongTapEvent(object source, Click click)
        {
            return new M5Event(KeyLongTap, source, new Dictionary<string, object>
            {
                { ContextSource, click }
            });
        }
    }

    public static class AnimationEvents
    {
        public const string KeyPlayStart = "OnAnimationPlayStart";
        public const string KeyPlayFinish = "OnAnimationPlayFinish";

        public const string ContextSource = "source";
        public const string ContextAnimationName = "animationName";

        public static M5Event CreatePlayStartEvent(object source, ModelRenderer renderer, string animationName)
        {
            return new M5Event(KeyPlayStart, source, new Dictionary<string, object>
            {
                { ContextSource,  renderer},
                { ContextAnimationName, animationName }
            });
        }

        public static M5Event CreatePlayFinishEvent(object source, ModelRenderer renderer, string animationName)
        {
            return new M5Event(KeyPlayFinish, source, new Dictionary<string, object>
            {
                { ContextSource,  renderer},
                { ContextAnimationName, animationName }
            });
        }
    }

    public static class VideoEvents
    {
        public const string KeyPlayStart = "OnVideoPlayStart";
        public const string KeyPlayPause = "OnVideoPlayPause";
        public const string KeyPlayStop = "OnVideoPlayStop";
        public const string KeyPlayFinish = "OnVideoPlayFinish";
        public const string KeyRecordFinish = "OnVideoRecordFinish";

        public const string ContextSource = "source";
        public const string ContextVideoUri = "videoUri";

        public static M5Event CreatePlayStartEvent(object source, VideoPlayerBase videoPlayerBase, string videoUri = null)
        {
            return new M5Event(KeyPlayStart, source, new Dictionary<string, object>
            {
                { ContextSource,  videoPlayerBase },
                { ContextVideoUri, videoUri }
            });
        }

        public static M5Event CreatePlayPauseEvent(object source, VideoPlayerBase videoPlayerBase, string videoUri = null)
        {
            return new M5Event(KeyPlayPause, source, new Dictionary<string, object>
            {
                { ContextSource,  videoPlayerBase },
                { ContextVideoUri, videoUri }
            });
        }

        public static M5Event CreatePlayStopEvent(object source, VideoPlayerBase videoPlayerBase, string videoUri = null)
        {
            return new M5Event(KeyPlayStop, source, new Dictionary<string, object>
            {
                { ContextSource,  videoPlayerBase },
                { ContextVideoUri, videoUri }
            });
        }

        public static M5Event CreatePlayFinishEvent(object source, VideoPlayerBase videoPlayerBase, string videoUri = null)
        {
            return new M5Event(KeyPlayFinish, source, new Dictionary<string, object>
            {
                { ContextSource,  videoPlayerBase },
                { ContextVideoUri, videoUri }
            });
        }

        public static M5Event CreateRecordFinishEvent(object source)
        {
            return new M5Event(KeyRecordFinish, source);
        }
    }

    public static class TrackerEvents
    {
        public const string KeyTargetTracked = "OnTargetTracked";

        public const string ContextSource = "source";
        public const string ContextTargetName = "targetName";
        public const string ContextPositionX = "posX";
        public const string ContextPositionY = "posY";
        public const string ContextPositionZ = "posZ";
        public const string ContextRotatioX = "rotX";
        public const string ContextRotatioY = "rotY";
        public const string ContextRotatioZ = "rotZ";

        public static M5Event CreateTargetTrackedEvent(object source, string targetName, Vector3 position, Quaternion rotation)
        {
            return new M5Event(KeyTargetTracked, source, new Dictionary<string, object>
            {
                { ContextTargetName, targetName },
                { ContextPositionX, position.x },
                { ContextPositionY, position.y },
                { ContextPositionZ, position.z },
                { ContextRotatioX, rotation.x },
                { ContextRotatioY, rotation.y },
                { ContextRotatioZ, rotation.z }
            });
        }
    }

    public static class TriggerEvents
    {
        public const string KeyEnter = "OnTriggerEnter";
        public const string KeyExit = "OnTriggerExit";

        public const string ContextSource = "source";
        public const string ContextOther = "other";

        public static M5Event CreateEnterEvent(object source, Collider col, Collider other)
        {
            return new M5Event(KeyEnter, source, new Dictionary<string, object>
            {
                { ContextSource, col },
                { ContextOther, other }
            });
        }

        public static M5Event CreateExitEvent(object source, Collider col, Collider other)
        {
            return new M5Event(KeyExit, source, new Dictionary<string, object>
            {
                { ContextSource, col },
                { ContextOther, other }
            });
        }
    }
}
