using System;

namespace Module5.Player
{
    public static class DefaultConverterRegistration
    {
        public static void RegisterDefaultConverters(IPersistentToGameObjectConverterRegistry registry)
        {
            if (registry == null) return;

            registry.RegisterConverter<PersistentAudioObject, PersistentToAudioObjectConverter>(GameObjectTypes.AudioSource);
            registry.RegisterConverter<PersistentBoxColliderObject, PersistentToBoxColliderObjectConverter>(GameObjectTypes.BoxCollider);
            registry.RegisterConverter<PersistentButtonObject, PersistentToButtonObjectConverter>(GameObjectTypes.Button);
            registry.RegisterConverter<PersistentCanvasObject, PersistentToCanvasObjectConverter>(GameObjectTypes.Canvas);
            registry.RegisterConverter<PersistentEventsObject, PersistentToEventObjectConverter>(GameObjectTypes.Event);
            registry.RegisterConverter<PersistentInputFieldObject, PersistentToInputFieldObjectConverter>(GameObjectTypes.InputField);
            registry.RegisterConverter<PersistentLightObject, PersistentToLightObjectConverter>(GameObjectTypes.Light);
            registry.RegisterConverter<PersistentModelObject, PersistentToModelObjectConverter>(GameObjectTypes.ModelRender);
            registry.RegisterConverter<PersistentPanelObject, PersistentToPanelObjectConverter>(GameObjectTypes.Panel);
            registry.RegisterConverter<PersistentRawImageObject, PersistentToRawImageObjectConverter>(GameObjectTypes.RawImage);
            registry.RegisterConverter<PersistentTextObject, PersistentToTextObjectConverter>(GameObjectTypes.Text);
            registry.RegisterConverter<PersistentVideoPlayer2DObject, PersistentToVideoPlayer2DObjectConverter>(GameObjectTypes.VideoPlayer2D);
            registry.RegisterConverter<PersistentVideoPlayer360Object, PersistentToVideoPlayer360ObjectConverter>(GameObjectTypes.VideoPlayer360);
            registry.RegisterConverter<PersistentVR360Object, PersistentToVR360ObjectConverter>(GameObjectTypes.VR360);
        }
    }
}
