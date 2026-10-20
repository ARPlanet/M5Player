using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    public static class NoneEvents
    {
        [EventDescriptor("none", "None")]
        public class NoneEvent : M5Event
        {
            public NoneEvent() : base(null) { }
        }
    }

    public static class AppEvents
    {
        [EventDescriptor("on_app_pause", "App Pause")]
        public class PauseEvent : M5Event
        {
            public PauseEvent(object source = default) : base(source) { }
        }

        [EventDescriptor("on_app_resume", "App Resume")]
        public class ResumeEvent : M5Event
        {
            public ResumeEvent(object source = default) : base(source) { }
        }
    }

    public static class SceneEvents
    {
        [EventDescriptor("on_scence_start", "Scene Start")]
        public class StartEvent : M5Event
        {
            public StartEvent(object source) : base(source) { }
        }

        [EventDescriptor("on_scene_exit", "Scene Exit")]
        public class ExitEvent : M5Event
        {
            public ExitEvent(object source) : base(source) { }
        }
    }

    public static class ObjectEvents
    {
        [EventDescriptor("on_object_enable", "Object Enable", typeof(GameObject))]
        public class EnableEvent : M5Event
        {
            public EnableEvent(object source) : base(source) { }
        }

        [EventDescriptor("on_object_disable", "Object Disable", typeof(GameObject))]
        public class DisableEvent : M5Event
        {
            public DisableEvent(object source) : base(source) { }
        }

        [EventDescriptor("on_object_click", "Object Click", typeof(Click))]
        public class ClickEvent : M5Event
        {
            [EventContext("source")]
            public Click Click { get; }

            public ClickEvent(object source, Click click) : base(source)
            {
                Click = click;
                Context["source"] = click;
            }
        }

        [EventDescriptor("on_object_longtap", "Object Long Tap", typeof(Click))]
        public class LongTapEvent : M5Event
        {
            [EventContext("source")]
            public Click Click { get; }

            public LongTapEvent(object source, Click click) : base(source)
            {
                Click = click;
                Context["source"] = click;
            }
        }
    }

    public static class AnimationEvents
    {
        [EventDescriptor("on_ani_play_start", "Animation Play Start", typeof(ModelRenderer))]
        public class PlayStartEvent : M5Event
        {
            [EventContext("source")]
            public ModelRenderer Renderer { get; }

            [EventContext("animationName")]
            public string AnimationName { get; }

            public PlayStartEvent(object source, ModelRenderer renderer, string animationName) : base(source)
            {
                Renderer = renderer;
                AnimationName = animationName;
                Context["source"] = renderer;
                Context["animationName"] = animationName;
            }
        }

        [EventDescriptor("on_ani_play_pause", "Animation Play Pause", typeof(ModelRenderer))]
        public class PlayPauseEvent : M5Event
        {
            [EventContext("source")]
            public ModelRenderer Renderer { get; }

            [EventContext("animationName")]
            public string AnimationName { get; }

            public PlayPauseEvent(object source, ModelRenderer renderer, string animationName) : base(source)
            {
                Renderer = renderer;
                AnimationName = animationName;
                Context["source"] = renderer;
                Context["animationName"] = animationName;
            }
        }

        [EventDescriptor("on_ani_play_stop", "Animation Play Stop", typeof(ModelRenderer))]
        public class PlayStopEvent : M5Event
        {
            [EventContext("source")]
            public ModelRenderer Renderer { get; }

            [EventContext("animationName")]
            public string AnimationName { get; }

            public PlayStopEvent(object source, ModelRenderer renderer, string animationName) : base(source)
            {
                Renderer = renderer;
                AnimationName = animationName;
                Context["source"] = renderer;
                Context["animationName"] = animationName;
            }
        }

        [EventDescriptor("on_ani_play_finish", "Animation Play Finish", typeof(ModelRenderer))]
        public class PlayFinishEvent : M5Event
        {
            [EventContext("source")]
            public ModelRenderer Renderer { get; }

            [EventContext("animationName")]
            public string AnimationName { get; }

            public PlayFinishEvent(object source, ModelRenderer renderer, string animationName) : base(source)
            {
                Renderer = renderer;
                AnimationName = animationName;
                Context["source"] = renderer;
                Context["animationName"] = animationName;
            }
        }

        [EventDescriptor("on_ani_play_loop_point_reached", "Animation Play Loop Point Reached", typeof(ModelRenderer))]
        public class LoopPointReachedEvent : M5Event
        {
            [EventContext("source")]
            public ModelRenderer Renderer { get; }

            [EventContext("animationName")]
            public string AnimationName { get; }

            public LoopPointReachedEvent(object source, ModelRenderer renderer, string animationName) : base(source)
            {
                Renderer = renderer;
                AnimationName = animationName;
                Context["source"] = renderer;
                Context["animationName"] = animationName;
            }
        }
    }

    public static class AudioEvents
    {
        [EventDescriptor("on_audio_play_start", "Audio Play Start", typeof(AudioPlayer))]
        public class PlayStartEvent : M5Event
        {
            [EventContext("source")]
            public AudioPlayer Player { get; }

            [EventContext("clipName")]
            public string ClipName { get; }

            public PlayStartEvent(object source, AudioPlayer player, string clipName = null) : base(source)
            {
                Player = player;
                ClipName = clipName;
                Context["source"] = player;
                Context["clipName"] = clipName;
            }
        }

        [EventDescriptor("on_audio_play_pause", "Audio Play Pause", typeof(AudioPlayer))]
        public class PlayPauseEvent : M5Event
        {
            [EventContext("source")]
            public AudioPlayer Player { get; }

            [EventContext("clipName")]
            public string ClipName { get; }

            public PlayPauseEvent(object source, AudioPlayer player, string clipName = null) : base(source)
            {
                Player = player;
                ClipName = clipName;
                Context["source"] = player;
                Context["clipName"] = clipName;
            }
        }

        [EventDescriptor("on_audio_play_stop", "Audio Play Stop", typeof(AudioPlayer))]
        public class PlayStopEvent : M5Event
        {
            [EventContext("source")]
            public AudioPlayer Player { get; }

            [EventContext("clipName")]
            public string ClipName { get; }

            public PlayStopEvent(object source, AudioPlayer player, string clipName = null) : base(source)
            {
                Player = player;
                ClipName = clipName;
                Context["source"] = player;
                Context["clipName"] = clipName;
            }
        }

        [EventDescriptor("on_audio_play_finish", "Audio Play Finish", typeof(AudioPlayer))]
        public class PlayFinishEvent : M5Event
        {
            [EventContext("source")]
            public AudioPlayer Player { get; }

            [EventContext("clipName")]
            public string ClipName { get; }

            public PlayFinishEvent(object source, AudioPlayer player, string clipName = null) : base(source)
            {
                Player = player;
                ClipName = clipName;
                Context["source"] = player;
                Context["clipName"] = clipName;
            }
        }

        [EventDescriptor("on_audio_play_loop_point_reached", "Audio Play Loop Point Reached", typeof(AudioPlayer))]
        public class LoopPointReachedEvent : M5Event
        {
            [EventContext("source")]
            public AudioPlayer Player { get; }

            [EventContext("clipName")]
            public string ClipName { get; }

            public LoopPointReachedEvent(object source, AudioPlayer player, string clipName = null) : base(source)
            {
                Player = player;
                ClipName = clipName;
                Context["source"] = player;
                Context["clipName"] = clipName;
            }
        }
    }

    public static class VideoEvents
    {
        [EventDescriptor("on_video_play_start", "Video Play Start", typeof(VideoPlayerBase))]
        public class PlayStartEvent : M5Event
        {
            [EventContext("source")]
            public VideoPlayerBase VideoPlayerBase { get; }

            [EventContext("videoUri")]
            public string VideoUri { get; }

            public PlayStartEvent(object source, VideoPlayerBase videoPlayerBase, string videoUri = null) : base(source)
            {
                VideoPlayerBase = videoPlayerBase;
                VideoUri = videoUri;
                Context["source"] = videoPlayerBase;
                Context["videoUri"] = videoUri;
            }
        }

        [EventDescriptor("on_video_play_pause", "Video Play Pause", typeof(VideoPlayerBase))]
        public class PlayPauseEvent : M5Event
        {
            [EventContext("source")]
            public VideoPlayerBase VideoPlayerBase { get; }

            [EventContext("videoUri")]
            public string VideoUri { get; }

            public PlayPauseEvent(object source, VideoPlayerBase videoPlayerBase, string videoUri = null) : base(source)
            {
                VideoPlayerBase = videoPlayerBase;
                VideoUri = videoUri;
                Context["source"] = videoPlayerBase;
                Context["videoUri"] = videoUri;
            }
        }

        [EventDescriptor("on_video_play_stop", "Video Play Stop", typeof(VideoPlayerBase))]
        public class PlayStopEvent : M5Event
        {
            [EventContext("source")]
            public VideoPlayerBase VideoPlayerBase { get; }

            [EventContext("videoUri")]
            public string VideoUri { get; }

            public PlayStopEvent(object source, VideoPlayerBase videoPlayerBase, string videoUri = null) : base(source)
            {
                VideoPlayerBase = videoPlayerBase;
                VideoUri = videoUri;
                Context["source"] = videoPlayerBase;
                Context["videoUri"] = videoUri;
            }
        }

        [EventDescriptor("on_video_play_finish", "Video Play Finish", typeof(VideoPlayerBase))]
        public class PlayFinishEvent : M5Event
        {
            [EventContext("source")]
            public VideoPlayerBase VideoPlayerBase { get; }

            [EventContext("videoUri")]
            public string VideoUri { get; }

            public PlayFinishEvent(object source, VideoPlayerBase videoPlayerBase, string videoUri = null) : base(source)
            {
                VideoPlayerBase = videoPlayerBase;
                VideoUri = videoUri;
                Context["source"] = videoPlayerBase;
                Context["videoUri"] = videoUri;
            }
        }

        [EventDescriptor("on_video_play_loop_point_reached", "Video Play Loop Point Reached", typeof(VideoPlayerBase))]
        public class LoopPointReachedEvent : M5Event
        {
            [EventContext("source")]
            public VideoPlayerBase VideoPlayerBase { get; }

            [EventContext("videoUri")]
            public string VideoUri { get; }

            public LoopPointReachedEvent(object source, VideoPlayerBase videoPlayerBase, string videoUri = null) : base(source)
            {
                VideoPlayerBase = videoPlayerBase;
                VideoUri = videoUri;
                Context["source"] = videoPlayerBase;
                Context["videoUri"] = videoUri;
            }
        }

        [EventDescriptor("on_video_record_finish", "Video Record Finish")]
        public class RecordFinishEvent : M5Event
        {
            public RecordFinishEvent(object source) : base(source) { }
        }
    }

    public static class TrackerEvents
    {
        [EventDescriptor("on_target_tracked", "Target Tracked", typeof(Anchor))]
        public class TargetTrackedEvent : M5Event
        {
            [EventContext("targetName")]
            public string TargetName { get; }

            [EventContext("posX")]
            public float PositionX { get; }

            [EventContext("posY")]
            public float PositionY { get; }

            [EventContext("posZ")]
            public float PositionZ { get; }

            [EventContext("rotX")]
            public float RotationX { get; }

            [EventContext("rotY")]
            public float RotationY { get; }

            [EventContext("rotZ")]
            public float RotationZ { get; }

            public TargetTrackedEvent(object source, string targetName, Vector3 position, Quaternion rotation) : base(source)
            {
                TargetName = targetName;
                PositionX = position.x;
                PositionY = position.y;
                PositionZ = position.z;
                RotationX = rotation.x;
                RotationY = rotation.y;
                RotationZ = rotation.z;

                Context["targetName"] = targetName;
                Context["posX"] = position.x;
                Context["posY"] = position.y;
                Context["posZ"] = position.z;
                Context["rotX"] = rotation.x;
                Context["rotY"] = rotation.y;
                Context["rotZ"] = rotation.z;
            }
        }
    }

    public static class TriggerEvents
    {
        [EventDescriptor("on_trigger_enter", "Trigger Enter", typeof(Collider))]
        public class EnterEvent : M5Event
        {
            [EventContext("source")]
            public Collider Col { get; }

            [EventContext("other")]
            public Collider Other { get; }

            public EnterEvent(object source, Collider col, Collider other) : base(source)
            {
                Col = col;
                Other = other;
                Context["source"] = col;
                Context["other"] = other;
            }
        }

        [EventDescriptor("on_trigger_exit", "Trigger Exit", typeof(Collider))]
        public class ExitEvent : M5Event
        {
            [EventContext("source")]
            public Collider Col { get; }

            [EventContext("other")]
            public Collider Other { get; }

            public ExitEvent(object source, Collider col, Collider other) : base(source)
            {
                Col = col;
                Other = other;
                Context["source"] = col;
                Context["other"] = other;
            }
        }
    }
}

