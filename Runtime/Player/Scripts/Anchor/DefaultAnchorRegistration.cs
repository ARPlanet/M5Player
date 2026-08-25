using System;

namespace Module5.Player
{
    public static class DefaultAnchorRegistration
    {
        public static void RegisterDefaultAnchors(IAnchorRegistry registry)
        {
            if (registry == null) return;

            registry.RegisterAnchorType<PersistentAnchorQr, PersistentToAnchorDataConverter>(AnchorQrData.Type);
            registry.RegisterAnchorType<PersistentAnchorImage, PersistentToAnchorImageDataConverter>(AnchorImageData.Type);
        }
    }
}
